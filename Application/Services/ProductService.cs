using Application.DTOs;
using Application.DTOs.Mappers;
using Domain.Models;
using Domain.Models.CategorizingModels;
using Domain.Models.ProductModels;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;

        public ProductService(IUnitOfWork unitOfWork, IWebHostEnvironment environment) // Add IMapper parameter
        {
            _environment = environment;
            _unitOfWork = unitOfWork;
        }

        private async Task<List<ProductPhoto>> SaveProductImagesAsync(IEnumerable<IFormFile> images, string uploadsFolder)
        {
            var productPhotos = new List<ProductPhoto>();

            if (images != null && images.Any())
            {
                foreach (var image in images)
                {
                    string uniqueFilename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFilename);
                    
                    await using var fileStream = new FileStream(filePath, FileMode.Create);
                    await image.CopyToAsync(fileStream);

                    productPhotos.Add(new ProductPhoto
                    {
                        Url = $"/Product/{uniqueFilename}",
                        Hash = GetPhotoHash(image)
                    });
                }
            }

            return productPhotos;
        }

        public async Task<int> CreateProductAsync(ProductDTO dto)
        {
            // Generate a unique folder name based on a GUID
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "Products");
            Directory.CreateDirectory(uploadsFolder);

            // Save the product images in the product folder
            var productPhotos = await SaveProductImagesAsync(dto.Image, uploadsFolder);

            // Create the product object and set its properties
            Product product = dto.DtoAsProductCreate(uploadsFolder); // Adjust if needed
            product.ProductPhotos = productPhotos;
            product.ProductCategories = dto.ProductCategories?.Select(id => new ProductCategory { CategoryId = id }).ToList() ?? new List<ProductCategory>();
            product.ProductFrames = dto.ProductFrames?.Select(id => new ProductFrame { FrameId = id }).ToList() ?? new List<ProductFrame>();
            product.ProductMaterials = dto.ProductMaterials?.Select(id => new ProductMaterial { MaterialId = id }).ToList() ?? new List<ProductMaterial>();
            product.ProductSizes = dto.ProductSizes?.Select(id => new ProductSize { SizeId = id }).ToList() ?? new List<ProductSize>();

            // Add the product to the database
            await _unitOfWork.Products.AddAsync(product);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync(
                 null,
                 q => q.Include(p => p.ProductCategories)
                       .ThenInclude(pc => pc.Category),
                 q => q.Include(p => p.ProductFrames)
                       .ThenInclude(pc => pc.Frame),
                 q => q.Include(p => p.ProductMaterials)
                 .ThenInclude(pc => pc.Material),
                 q => q.Include(p => p.ProductSizes)
                 .ThenInclude(pc => pc.Size),
                 q => q.Include(p => p.ProductPhotos)
            );
            return products.Select(p => p.ProductAsDto()).ToList();
        }

        public async Task<int> UpdateProductAsync(ProductDTO dto)
        {
            var product = await _unitOfWork.Products.GetAsync(
                p => p.Id == dto.Id,
                includeProperties: query => query
                    .Include(p => p.ProductCategories)
                    .Include(p => p.ProductFrames)
                    .Include(p => p.ProductMaterials)
                    .Include(p => p.ProductSizes)
                    .Include(p => p.ProductPhotos)
            );

            // Ensure ProductCategories is initialized
            product.ProductCategories ??= new List<ProductCategory>();
            product.ProductFrames ??= new List<ProductFrame>();
            product.ProductMaterials ??= new List<ProductMaterial>();
            product.ProductSizes ??= new List<ProductSize>();
            product.ProductPhotos ??= new List<ProductPhoto>();

            // Safely retrieve current categories
            var currentCategories = product.ProductCategories.Select(pc => pc.CategoryId).ToHashSet();
            var currentFrames = product.ProductFrames.Select(pf => pf.FrameId).ToHashSet();
            var currentMaterials = product.ProductMaterials.Select(pm => pm.MaterialId).ToHashSet();
            var currentSizes = product.ProductSizes.Select(ps => ps.SizeId).ToHashSet();
            var currentPhotos = product.ProductPhotos.ToDictionary(pp => pp.Hash, pp => pp);

            // Handle null for dto.ProductCategories
            var newCategories = dto.ProductCategories ?? new List<int>();
            var newFrames = dto.ProductFrames ?? new List<int>();
            var newMaterials = dto.ProductMaterials ?? new List<int>();
            var newSizes = dto.ProductSizes ?? new List<int>();
            var newPhotos = dto.Image?.Select(i => (i, GetPhotoHash(i))).ToList() ?? new List<(IFormFile, string)>();

            // Find categories to add and remove
            var categoriesToAdd = newCategories.Where(nc => !currentCategories.Contains(nc)).ToList();
            var categoriesToRemove = currentCategories.Where(cc => !newCategories.Contains(cc)).ToList();
            var framesToAdd = newFrames.Where(nf => !currentFrames.Contains(nf)).ToList();
            var framesToRemove = currentFrames.Where(cf => !newFrames.Contains(cf)).ToList();
            var materialsToAdd = newMaterials.Where(nm => !currentMaterials.Contains(nm)).ToList();
            var materialsToRemove = currentMaterials.Where(cm => !newMaterials.Contains(cm)).ToList();
            var sizesToAdd = newSizes.Where(ns => !currentSizes.Contains(ns)).ToList();
            var sizesToRemove = currentSizes.Where(cs => !newSizes.Contains(cs)).ToList();

            // Find photos to add and remove based on hash
            var photosToAdd = newPhotos.Where(np => !currentPhotos.ContainsKey(np.Item2)).ToList();
            var photosToRemove = currentPhotos.Values.Where(cp => !newPhotos.Any(np => np.Item2 == cp.Hash)).ToList();

            // Remove old product-category relationships
            var productCategoriesToRemove = product.ProductCategories
                .Where(pc => categoriesToRemove.Contains(pc.CategoryId))
                .ToList();

            foreach (var category in productCategoriesToRemove)
            {
                product.ProductCategories.Remove(category);
            }

            // Add new product-category relationships
            foreach (var categoryId in categoriesToAdd)
            {
                product.ProductCategories.Add(new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = categoryId
                });
            }

            // Remove old product-frame relationships
            var productFramesToRemove = product.ProductFrames
                .Where(pf => framesToRemove.Contains(pf.FrameId))
                .ToList();

            foreach (var frame in productFramesToRemove)
            {
                product.ProductFrames.Remove(frame);
            }

            // Add new product-frame relationships
            foreach (var frameId in framesToAdd)
            {
                product.ProductFrames.Add(new ProductFrame
                {
                    ProductId = product.Id,
                    FrameId = frameId
                });
            }

            // Remove old product-material relationships
            var productMaterialsToRemove = product.ProductMaterials
                .Where(pm => materialsToRemove.Contains(pm.MaterialId))
                .ToList();

            foreach (var material in productMaterialsToRemove)
            {
                product.ProductMaterials.Remove(material);
            }

            // Add new product-material relationships
            foreach (var materialId in materialsToAdd)
            {
                product.ProductMaterials.Add(new ProductMaterial
                {
                    ProductId = product.Id,
                    MaterialId = materialId
                });
            }

            // Remove old product-size relationships
            var productSizesToRemove = product.ProductSizes
                .Where(ps => sizesToRemove.Contains(ps.SizeId))
                .ToList();

            foreach (var size in productSizesToRemove)
            {
                product.ProductSizes.Remove(size);
            }

            // Add new product-size relationships
            foreach (var sizeId in sizesToAdd)
            {
                product.ProductSizes.Add(new ProductSize
                {
                    ProductId = product.Id,
                    SizeId = sizeId
                });
            }

            // Remove old product-photo relationships
            foreach (var photo in photosToRemove)
            {
                product.ProductPhotos.Remove(photo);
                var oldImagePath = Path.Combine(_environment.WebRootPath, photo.Url.TrimStart('/'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            // Add new product-photo relationships
            foreach (var (photo, hash) in photosToAdd)
            {
                string uniqueFilename = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "Product", uniqueFilename);

                var directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await photo.CopyToAsync(fileStream);

                product.ProductPhotos.Add(new ProductPhoto
                {
                    Url = $"/Product/{uniqueFilename}",
                    Hash = hash
                });
            }

            // Save changes
            await _unitOfWork.Products.UpdateAsync(product);
            return await _unitOfWork.CommitAsync();
        }

        // Helper method to get the hash of a photo
        private string GetPhotoHash(IFormFile photo)
        {
            using var md5 = MD5.Create();
            using var stream = photo.OpenReadStream();
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public async Task<int> DeleteProductAsync(ProductDTO dto)
        {
            // Fetch the product to be deleted
            var product = await _unitOfWork.Products.GetAsync(
                p => p.Id == dto.Id,
                includeProperties: query => query
                    .Include(p => p.ProductCategories)
                    .Include(p => p.ProductFrames)
                    .Include(p => p.ProductMaterials)
                    .Include(p => p.ProductSizes)
                    .Include(p => p.ProductPhotos)
            );

            if (product == null)
            {
                return 0; // Product not found
            }

            // Remove product-category relationships
            foreach (var category in product.ProductCategories)
            {
                product.ProductCategories.Remove(category);
            }

            // Remove product-frame relationships
            foreach (var frame in product.ProductFrames)
            {
                product.ProductFrames.Remove(frame);
            }

            // Remove product-material relationships
            foreach (var material in product.ProductMaterials)
            {
                product.ProductMaterials.Remove(material);
            }

            // Remove product-size relationships
            foreach (var size in product.ProductSizes)
            {
                product.ProductSizes.Remove(size);
            }

            // Remove product-photo relationships and delete associated files
            foreach (var photo in product.ProductPhotos)
            {
                product.ProductPhotos.Remove(photo);
                var oldImagePath = Path.Combine(_environment.WebRootPath, photo.Url.TrimStart('/'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            // Remove the product itself
            await _unitOfWork.Products.DeleteAsync(product);

            // Save changes
            return await _unitOfWork.CommitAsync();
        }
    }
}