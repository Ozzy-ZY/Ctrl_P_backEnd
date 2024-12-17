using Application.DTOs;
using Application.DTOs.Mappers;
using Domain.Models;
using Domain.Models.CategorizingModels;
using Domain.Models.ProductModels;
using Domain.Models.ShopModels;
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
        private readonly WishlistService _wishlistService;

        public ProductService(IUnitOfWork unitOfWork, IWebHostEnvironment environment, WishlistService wishlistService) // Add IMapper parameter
        {
            _environment = environment;
            _unitOfWork = unitOfWork;
            _wishlistService = wishlistService;
        }



        public async Task<ServiceResult> CreateProductAsync(ProductDTOCreate dto)
        {
            ServiceResult result = new ServiceResult();

            // Generate a unique folder name based on a GUID
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "Product");
            Directory.CreateDirectory(uploadsFolder);

            // Save the product images in the product folder
            var productPhotos = await SaveProductImagesAsync(dto.Image, uploadsFolder);

            // Create product object
            Product product = dto.DtoAsProductCreate(uploadsFolder);
            product.ProductPhotos = productPhotos;

            // Resolve and map categories
            var categoryEntities = await _unitOfWork.Categories.GetAllAsync(c => dto.CategoryNames.Contains(c.Name));
            product.ProductCategories = categoryEntities
                .Select(c => new ProductCategory { CategoryId = c.Id, Product = product })
                .ToList();

            // Resolve and map frames
            var frameEntities = await _unitOfWork.Frames.GetAllAsync(f => dto.FramesNames.Contains(f.Name));
            product.ProductFrames = frameEntities
                .Select(f => new ProductFrame { FrameId = f.Id, Product = product })
                .ToList();

            // Resolve and map materials
            var materialEntities = await _unitOfWork.Materials.GetAllAsync(m => dto.MaterialsNames.Contains(m.Name));
            product.ProductMaterials = materialEntities
                .Select(m => new ProductMaterial { MaterialId = m.Id, Product = product })
                .ToList();

            // Resolve and map sizes
            var sizeEntities = await _unitOfWork.Sizes.GetAllAsync(s => dto.SizesNames.Contains(s.Name));
            product.ProductSizes = sizeEntities
                .Select(s => new ProductSize { SizeId = s.Id, Product = product })
                .ToList();

            // Add the product to the database
            await _unitOfWork.Products.AddAsync(product);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                result.Success = true;
                return result;
            }

            result.Errors.Add("Couldn't add the product");
            result.Success = false;
            return result;
        }




        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int? userId)
        {
            var products = await _unitOfWork.Products.GetAllAsync(
                 null,
                 q => q.Include(p => p.ProductPhotos.OrderBy(photo => photo.Id).Take(1)),
                 q => q.Include(p => p.ProductSizes.OrderBy(f=>f.SizeId).Take(1))
                       .ThenInclude(pc => pc.Size));

            var productDtos = products.Select(p =>
            {
                var productDto = p.ProductAsDto();
                if (userId.HasValue)
                {
                    productDto = productDto with { IsInWishlist = _wishlistService.IsProductInWishlistAsync(userId.Value, productDto.Id).Result };
                }
                return productDto;
            });

            return productDtos;
        }
        public async Task<ProductDTO> GetProductAsync(int Id, int? userId)
        {
            var product = await _unitOfWork.Products.GetAsync(
                p => p.Id == Id,
                includeProperties: query => query
                    .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                    .Include(p => p.ProductFrames)
                    .ThenInclude(pf => pf.Frame)
                    .Include(p => p.ProductMaterials)
                    .ThenInclude(pm => pm.Material)
                    .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                    .Include(p => p.ProductPhotos)
                    .Include(p => p.ProductReviews)
            );

            var productDto = product.ProductAsDto();

            if (userId.HasValue)
            {
                productDto = productDto with { IsInWishlist = await _wishlistService.IsProductInWishlistAsync(userId.Value, productDto.Id) };
            }

            return productDto;
        }

        public async Task<ServiceResult> UpdateProductAsync(ProductDTOCreate dto)
        {
            ServiceResult result = new ServiceResult();

            // Retrieve the existing product with its related entities
            var product = await _unitOfWork.Products.GetAsync(
                p => p.Id == dto.Id,
                includeProperties: query => query
                    .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                    .Include(p => p.ProductFrames)
                    .ThenInclude(pf => pf.Frame)
                    .Include(p => p.ProductMaterials)
                    .ThenInclude(pm => pm.Material)
                    .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                    .Include(p => p.ProductPhotos)
            );

            if (product == null)
            {
                result.Errors.Add("Product not found");
                result.Success = false;
                return result;
            }

            // Initialize lists if null
            product.ProductCategories ??= new List<ProductCategory>();
            product.ProductFrames ??= new List<ProductFrame>();
            product.ProductMaterials ??= new List<ProductMaterial>();
            product.ProductSizes ??= new List<ProductSize>();
            product.ProductPhotos ??= new List<ProductPhoto>();

            // Fetch related entities by name
            var categories = await _unitOfWork.Categories.GetAllAsync(c => dto.CategoryNames.Contains(c.Name));
            var frames = await _unitOfWork.Frames.GetAllAsync(f => dto.FramesNames.Contains(f.Name));
            var materials = await _unitOfWork.Materials.GetAllAsync(m => dto.MaterialsNames.Contains(m.Name));
            var sizes = await _unitOfWork.Sizes.GetAllAsync(s => dto.SizesNames.Contains(s.Name));

            // Update ProductCategories
            UpdateProductRelationships(
                product.ProductCategories,
                categories.Select(c => c.Id).ToHashSet(),
                pc => pc.CategoryId,
                id => new ProductCategory { ProductId = product.Id, CategoryId = id }
            );

            // Update ProductFrames
            UpdateProductRelationships(
                product.ProductFrames,
                frames.Select(f => f.Id).ToHashSet(),
                pf => pf.FrameId,
                id => new ProductFrame { ProductId = product.Id, FrameId = id }
            );

            // Update ProductMaterials
            UpdateProductRelationships(
                product.ProductMaterials,
                materials.Select(m => m.Id).ToHashSet(),
                pm => pm.MaterialId,
                id => new ProductMaterial { ProductId = product.Id, MaterialId = id }
            );

            // Update ProductSizes
            UpdateProductRelationships(
                product.ProductSizes,
                sizes.Select(s => s.Id).ToHashSet(),
                ps => ps.SizeId,
                id => new ProductSize { ProductId = product.Id, SizeId = id }
            );

            // Update ProductPhotos
            await UpdateProductPhotosAsync(product, dto.Image);
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.OldPrice = dto.OldPrice;
            product.InStockAmount = dto.UnitsInStock;

            // Save changes
            await _unitOfWork.Products.UpdateAsync(product);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                result.Success = true;
                return result;
            }

            result.Errors.Add("Couldn't update the product");
            result.Success = false;
            return result;
        }

        // Helper method to update many-to-many relationships
        private void UpdateProductRelationships<T>(
            ICollection<T> existingItems,
            HashSet<int> newIds,
            Func<T, int> idSelector,
            Func<int, T> createNewItem)
        {
            var existingIds = existingItems.Select(idSelector).ToHashSet();

            // Find items to add and remove
            var idsToAdd = newIds.Except(existingIds).ToList();
            var itemsToRemove = existingItems.Where(e => !newIds.Contains(idSelector(e))).ToList();

            // Remove old relationships
            foreach (var item in itemsToRemove)
            {
                existingItems.Remove(item);
            }

            // Add new relationships
            foreach (var id in idsToAdd)
            {
                existingItems.Add(createNewItem(id));
            }
        }

        // Helper method to update product photos
        private async Task UpdateProductPhotosAsync(Product product, IEnumerable<IFormFile>? images)
        {
            if (images == null) return;

            var currentPhotos = product.ProductPhotos.ToDictionary(pp => pp.Hash, pp => pp);

            // Generate new photos with hashes
            var newPhotos = images.Select(image => (Image: image, Hash: GetPhotoHash(image))).ToList();

            // Find photos to add and remove
            var photosToAdd = newPhotos.Where(np => !currentPhotos.ContainsKey(np.Hash)).ToList();
            var photosToRemove = currentPhotos.Values.Where(cp => !newPhotos.Any(np => np.Hash == cp.Hash)).ToList();

            // Remove old photos
            foreach (var photo in photosToRemove)
            {
                product.ProductPhotos.Remove(photo);
                var oldImagePath = Path.Combine(_environment.WebRootPath, photo.Url.TrimStart('/'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            // Add new photos
            foreach (var (image, hash) in photosToAdd)
            {
                string uniqueFilename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "Product", uniqueFilename);

                var directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(fileStream);

                product.ProductPhotos.Add(new ProductPhoto
                {
                    Url = $"/Product/{uniqueFilename}",
                    Hash = hash
                });
            }
        }

            public async Task<ServiceResult> DeleteProductAsync(ProductDTO dto)
        {
            ServiceResult result = new ServiceResult();
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
            if (await _unitOfWork.CommitAsync() > 0)
            {
                result.Success = true;
                return result;
            }

            result.Errors.Add("Couldn't delete the product");
            result.Success = false;
            return result;
        }
        // Helper method to get the hash of a photo
        private string GetPhotoHash(IFormFile photo)
        {
            using var md5 = MD5.Create();
            using var stream = photo.OpenReadStream();
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
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
    }
}