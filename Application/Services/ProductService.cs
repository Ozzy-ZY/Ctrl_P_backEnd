using Application.DTOs;
using Application.DTOs.Mappers;
using Application.Services;
using Domain.Models;
using Domain.Models.CategorizingModels;
using Domain.Models.ProductModels;
using Domain.Models.ShopModels;
using Infrastructure.DataAccess;
using Infrastructure.DataAccess.Repositories;
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
            var result = new ServiceResult();

            // Validate input DTO
            if (dto == null)
            {
                result.Errors.Add("Product data is null.");
                return result;
            }

            if (string.IsNullOrEmpty(dto.Name))
            {
                result.Errors.Add("Product name is required.");
                return result;
            }

            // Validate categories, frames, materials, and sizes
            if (dto.CategoryNames == null || !dto.CategoryNames.Any())
            {
                result.Errors.Add("At least one category is required.");
                return result;
            }

            // Ensure the uploads folder exists
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "Product");
            if (string.IsNullOrEmpty(_environment.WebRootPath) || !Directory.Exists(_environment.WebRootPath))
            {
                result.Errors.Add("Web root path is invalid.");
                return result;
            }
            Directory.CreateDirectory(uploadsFolder);

            // Save the product images
            var productPhotos = await SaveProductImagesAsync(dto.Image, uploadsFolder);
            if (productPhotos == null || !productPhotos.Any())
            {
                result.Errors.Add("Failed to save product images.");
                return result;
            }

            // Map DTO to Product
            var product = dto.DtoAsProductCreate(uploadsFolder);
            if (product == null)
            {
                result.Errors.Add("Failed to map product data.");
                return result;
            }

            product.ProductPhotos = productPhotos;

            // Resolve categories
            var categoryEntities = await _unitOfWork.Categories.GetAllAsync(c => dto.CategoryNames.Contains(c.Name));
            if (categoryEntities == null || !categoryEntities.Any())
            {
                result.Errors.Add("Invalid categories provided.");
                return result;
            }

            product.ProductCategories = categoryEntities
                .Select(c => new ProductCategory { CategoryId = c.Id, Product = product })
                .ToList();

            // Resolve frames
            var frameEntities = await _unitOfWork.Frames.GetAllAsync(f => dto.FramesNames.Contains(f.Name));
            if (frameEntities == null || !frameEntities.Any())
            {
                result.Errors.Add("Invalid frames provided.");
                return result;
            }

            product.ProductFrames = frameEntities
                .Select(f => new ProductFrame { FrameId = f.Id, Product = product })
                .ToList();

            // Resolve materials
            var materialEntities = await _unitOfWork.Materials.GetAllAsync(m => dto.MaterialsNames.Contains(m.Name));
            if (materialEntities == null || !materialEntities.Any())
            {
                result.Errors.Add("Invalid materials provided.");
                return result;
            }

            product.ProductMaterials = materialEntities
                .Select(m => new ProductMaterial { MaterialId = m.Id, Product = product })
                .ToList();

            // Resolve sizes
            var sizeEntities = await _unitOfWork.Sizes.GetAllAsync(s => dto.SizesNames.Contains(s.Name));
            if (sizeEntities == null || !sizeEntities.Any())
            {
                result.Errors.Add("Invalid sizes provided.");
                return result;
            }

            product.ProductSizes = sizeEntities
                .Select(s => new ProductSize { SizeId = s.Id, Product = product })
                .ToList();

            // Add product to the database
            await _unitOfWork.Products.AddAsync(product);

            if (await _unitOfWork.CommitAsync() <= 0)
            {
                result.Errors.Add("Failed to save the product to the database.");
                return result;
            }

            result.Success = true;
            return result;
        }



        private async Task<IEnumerable<TEntity>> ValidateAndFetchEntities<TEntity>(
    IEnumerable<string> names,
    IGenericRepository<TEntity> repository,
    string entityType,
    ServiceResult result) where TEntity : class
        {
            if (names == null || !names.Any())
            {
                result.Errors.Add($"{entityType} names are missing");
                return Enumerable.Empty<TEntity>();
            }

            // Ensure TEntity has a property named "Name"
            var nameProperty = typeof(TEntity).GetProperty("Name");
            if (nameProperty == null || nameProperty.PropertyType != typeof(string))
            {
                result.Errors.Add($"{entityType} does not have a valid 'Name' property.");
                return Enumerable.Empty<TEntity>();
            }

            // Query using EF Core without reflection
            var entities = await repository.GetAllAsync(
                e => names.Contains(EF.Property<string>(e, "Name"))
            );

            // Validate the count
            if (entities == null || entities.Count() != names.Count())
            {
                var foundNames = entities.Select(e => EF.Property<string>(e, "Name"));
                var missing = names.Except(foundNames);
                result.Errors.Add($"The following {entityType}(s) were not found: {string.Join(", ", missing)}");
            }

            return entities;
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

        public async Task<ServiceResult> UpdateProductAsync(ProductDTOUpdate dto)
        {
            ServiceResult result = new ServiceResult();

            // Step 1: Retrieve the existing product with its related entities
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

            // Step 2: Validate and fetch related entities
            var categories = await ValidateAndFetchEntities(dto.CategoryNames, _unitOfWork.Categories, "Category", result);
            var frames = await ValidateAndFetchEntities(dto.FramesNames, _unitOfWork.Frames, "Frame", result);
            var materials = await ValidateAndFetchEntities(dto.MaterialsNames, _unitOfWork.Materials, "Material", result);
            var sizes = await ValidateAndFetchEntities(dto.SizesNames, _unitOfWork.Sizes, "Size", result);

            if (result.Errors.Any())
            {
                result.Success = false;
                return result;
            }

            // Step 3: Update relationships (only if valid data is provided)
            if (categories != null && categories.Any())
            {
                UpdateProductRelationships(
                    product.ProductCategories,
                    categories.Select(c => c.Id).ToHashSet(),
                    pc => pc.CategoryId,
                    id => new ProductCategory { ProductId = product.Id, CategoryId = id }
                );
            }

            if (frames != null && frames.Any())
            {
                UpdateProductRelationships(
                    product.ProductFrames,
                    frames.Select(f => f.Id).ToHashSet(),
                    pf => pf.FrameId,
                    id => new ProductFrame { ProductId = product.Id, FrameId = id }
                );
            }

            if (materials != null && materials.Any())
            {
                UpdateProductRelationships(
                    product.ProductMaterials,
                    materials.Select(m => m.Id).ToHashSet(),
                    pm => pm.MaterialId,
                    id => new ProductMaterial { ProductId = product.Id, MaterialId = id }
                );
            }

            if (sizes != null && sizes.Any())
            {
                UpdateProductRelationships(
                    product.ProductSizes,
                    sizes.Select(s => s.Id).ToHashSet(),
                    ps => ps.SizeId,
                    id => new ProductSize { ProductId = product.Id, SizeId = id }
                );
            }

            // Step 4: Update product photos
            try
            {
                await UpdateProductPhotosAsync(product, dto.Image);
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Failed to update product photos: {ex.Message}");
                result.Success = false;
                return result;
            }

            // Step 5: Update other product details (only if valid data is provided)
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                product.Name = dto.Name;
            }

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                product.Description = dto.Description;
            }

            if (dto.Price > 0)
            {
                product.Price = dto.Price;
            }

            if (dto.OldPrice != null && dto.OldPrice > 0)
            {
                product.OldPrice = dto.OldPrice;
            }

            if (dto.UnitsInStock != null && dto.UnitsInStock > 0)
            {
                product.InStockAmount = dto.UnitsInStock;
            }

            // Step 6: Save changes
            await _unitOfWork.Products.UpdateAsync(product);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                result.Success = true;
                return result;
            }

            result.Errors.Add("Couldn't save the updated product");
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

            // Map current photos by their hash
            var currentPhotosGrouped = product.ProductPhotos
                .GroupBy(pp => pp.Hash)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Generate hashes for new images and remove duplicates
            var newPhotosGrouped = images
                .Where(image => image != null)
                .Select(image => (Image: image, Hash: GetPhotoHash(image)))
                .GroupBy(np => np.Hash)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Ensure the product directory exists
            var directoryPath = Path.Combine(_environment.WebRootPath, "Product");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Process each hash in the current photos
            foreach (var currentHash in currentPhotosGrouped.Keys.ToList())
            {
                currentPhotosGrouped.TryGetValue(currentHash, out var currentPhotos);
                newPhotosGrouped.TryGetValue(currentHash, out var newPhotosWithSameHash);

                // Count existing and new photos with the same hash
                var currentCount = currentPhotos?.Count ?? 0;
                var newCount = newPhotosWithSameHash?.Count ?? 0;

                if (newCount > currentCount)
                {
                    // Add more photos to match the count
                    foreach (var (image, hash) in newPhotosWithSameHash.Skip(currentCount))
                    {
                        try
                        {
                            string uniqueFilename = Guid.NewGuid() + Path.GetExtension(image.FileName);
                            var filePath = Path.Combine(directoryPath, uniqueFilename);

                            await using var fileStream = new FileStream(filePath, FileMode.Create);
                            await image.CopyToAsync(fileStream);

                            product.ProductPhotos.Add(new ProductPhoto
                            {
                                Url = $"/Product/{uniqueFilename}",
                                Hash = hash
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Error] Failed to add photo {image.FileName}: {ex.Message}");
                        }
                    }
                }
                else if (newCount < currentCount)
                {
                    // Remove excess photos to match the count
                    var photosToRemove = currentPhotos.Take(currentCount - newCount).ToList();
                    foreach (var photo in photosToRemove)
                    {
                        try
                        {
                            product.ProductPhotos.Remove(photo);
                            var oldImagePath = Path.Combine(_environment.WebRootPath, photo.Url.TrimStart('/'));
                            if (File.Exists(oldImagePath))
                            {
                                File.Delete(oldImagePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting file {photo.Url}: {ex.Message}");
                        }
                    }
                }
                // Do nothing if the counts are equal
            }

            // Process new photos with hashes not in current photos
            foreach (var (hash, newPhotosWithSameHash) in newPhotosGrouped)
            {
                if (!currentPhotosGrouped.ContainsKey(hash))
                {
                    foreach (var (image, _) in newPhotosWithSameHash)
                    {
                        try
                        {
                            string uniqueFilename = Guid.NewGuid() + Path.GetExtension(image.FileName);
                            var filePath = Path.Combine(directoryPath, uniqueFilename);

                            await using var fileStream = new FileStream(filePath, FileMode.Create);
                            await image.CopyToAsync(fileStream);

                            product.ProductPhotos.Add(new ProductPhoto
                            {
                                Url = $"/Product/{uniqueFilename}",
                                Hash = hash
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Error] Failed to add photo {image.FileName}: {ex.Message}");
                        }
                    }
                }
            }
        }


        public async Task<ServiceResult> DeleteProductAsync(int ProductId)

        {
            ServiceResult result = new ServiceResult();
            // Fetch the product to be deleted
            var product = await _unitOfWork.Products.GetAsync(
                p => p.Id == ProductId,
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
        public async Task<IEnumerable<ProductDTO>> FilterProductsAsync(
            IEnumerable<int>? categoryIds = null,
            IEnumerable<int>? frameIds = null,
            IEnumerable<int>? materialIds = null,
            IEnumerable<int>? sizeIds = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minRating = null,
            int? maxRating = null,
            string? nameContains = null,
            int? userId = null)
        {
            var query = _unitOfWork.Products.Query();

            // Filter by category
            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(p => p.ProductCategories.Any(pc => categoryIds.Contains(pc.CategoryId)));
            }

            // Filter by frame
            if (frameIds != null && frameIds.Any())
            {
                query = query.Where(p => p.ProductFrames.Any(pf => frameIds.Contains(pf.FrameId)));
            }

            // Filter by material
            if (materialIds != null && materialIds.Any())
            {
                query = query.Where(p => p.ProductMaterials.Any(pm => materialIds.Contains(pm.MaterialId)));
            }

            // Filter by size
            if (sizeIds != null && sizeIds.Any())
            {
                query = query.Where(p => p.ProductSizes.Any(ps => sizeIds.Contains(ps.SizeId)));
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Filter by rating
            if (minRating.HasValue)
            {
                query = query.Where(p => p.Rating >= minRating.Value);
            }
            if (maxRating.HasValue)
            {
                query = query.Where(p => p.Rating <= maxRating.Value);
            }

            // Filter by product name
            if (!string.IsNullOrEmpty(nameContains))
            {
                query = query.Where(p => p.Name.Contains(nameContains));
            }

            // Include related data
            var products = await query
                .Include(p => p.ProductPhotos.OrderBy(photo => photo.Id).Take(1))
                .Include(p => p.ProductSizes.OrderBy(f => f.SizeId).Take(1))
                    .ThenInclude(pc => pc.Size)
                .ToListAsync();

            // Map to DTOs and include wishlist info if userId is provided
            var productDtos = products.Select(p =>
            {
                var dto = p.ProductAsDto();
                if (userId.HasValue)
                {
                    dto = dto with { IsInWishlist = _wishlistService.IsProductInWishlistAsync(userId.Value, dto.Id).Result };
                }
                return dto;
            });

            return productDtos;
        }
    }
}