using Application.DTOs.CategorizingModels;
using Domain.Models;
using Domain.Models.CategorizingModels;
using Domain.Models.ProductModels;
using System;
using System.Net.NetworkInformation;

namespace Application.DTOs.Mappers
{
    public static class ProductMapper
    {
        public static Product DtoAsProductCreate(this ProductDTOCreate dto, string url)
        {
            return new Product()
            {
                Name = dto.Name,
                Description = dto.Description,
                InStock = dto.UnitsInStock > 0 ? true : false,
                InStockAmount = dto.UnitsInStock,
                Price = dto.Price,
                OldPrice = dto.OldPrice,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                // Map Categories
                ProductCategories = (dto.CategoryNames ?? Enumerable.Empty<string>())
            .Select(name => new ProductCategory { Category = new Category { Name = name } })
            .ToList(),

                // Map Frames
                ProductFrames = (dto.FramesNames ?? Enumerable.Empty<string>())
            .Select(name => new ProductFrame { Frame = new Frame { Name = name } })
            .ToList(),

                // Map Materials
                ProductMaterials = (dto.MaterialsNames ?? Enumerable.Empty<string>())
            .Select(name => new ProductMaterial { Material = new Material { Name = name } })
            .ToList(),

                // Map Sizes
                ProductSizes = (dto.SizesNames ?? Enumerable.Empty<string>())
            .Select(name => new ProductSize { Size = new Size { Name = name } })
            .ToList(),
                ProductPhotos = new List<ProductPhoto> { new ProductPhoto { Url = url } }
            };
        }

        public static Product DtoAsProductUpdate(this ProductDTO dto, string url)
        {
            return new Product()
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                InStock = dto.UnitsInStock > 0 ? true : false,
                InStockAmount = dto.UnitsInStock,
                Price = dto.Price,
                OldPrice = dto.OldPrice,
                UpdatedAt = DateTime.Now,
                ProductCategories = (dto.ProductCategoryIds ?? Enumerable.Empty<int>()).Select(id => new ProductCategory
                {
                    CategoryId = id
                }).ToList(),
                ProductFrames = (dto.ProductFrameIds ?? Enumerable.Empty<int>()).Select(id => new ProductFrame
                {
                    FrameId = id
                }).ToList(),
                ProductMaterials = (dto.ProductMaterialIds ?? Enumerable.Empty<int>()).Select(id => new ProductMaterial
                {
                    MaterialId = id
                }).ToList(),
                ProductSizes = (dto.ProductSizeIds ?? Enumerable.Empty<int>()).Select(id => new ProductSize
                {
                    SizeId = id
                }).ToList(),
                ProductPhotos = new List<ProductPhoto> { new ProductPhoto { Url = url } }
            };
        }

        public static ProductDTO ProductAsDto(this Product product)
        {
            return new ProductDTO(
                Id: product.Id,
                Name: product.Name,
                Description: product.Description,
                UnitsInStock: product.InStockAmount,
                Price: product.Price,
                OldPrice: product.OldPrice,
                Rating: product.Rating,
                ProductCategoryIds: product.ProductCategories?
                    .Where(pc => pc.CategoryId != 0)
                    .Select(pc => pc.CategoryId)
                    .ToList() ?? new List<int>(),
                CategoryNames: product.ProductCategories?
                    .Select(pc => pc.Category.Name)
                    .ToList(),
                ProductFrameIds: product.ProductFrames?
                    .Where(pf => pf.FrameId != 0)
                    .Select(pf => pf.FrameId)
                    .ToList() ?? new List<int>(),
                FramesNames: product.ProductFrames?
                    .Select(pf => pf.Frame.Name)
                    .ToList(),
                ProductMaterialIds: product.ProductMaterials?
                    .Where(pm => pm.MaterialId != 0)
                    .Select(pm => pm.MaterialId)
                    .ToList() ?? new List<int>(),
                MaterialsNames: product.ProductMaterials?
                    .Select(pm => pm.Material.Name)
                    .ToList(),
                ProductSizeIds: product.ProductSizes?
                    .Where(ps => ps.SizeId != 0)
                    .Select(ps => ps.SizeId)
                    .ToList(),
                SizesNames: product.ProductSizes?
                    .Select(ps => ps.Size.Name)
                    .ToList() ,
                Url: product.ProductPhotos?
                    .Select(pp => pp.Url)
                    .ToList() ?? new List<string>(),
                Image: null!,
                Reviews: product.ProductReviews?
            .Select(pr => new ProductReviewsDto(
                Id: pr.Id,
                Review: pr.Review,
                ReviewerName: pr.Name,
                ReviewerEmail: pr.Email,
                Rating: pr.Rating,
                ReviewDate: pr.ReviewDate,
                ReviewerId: pr.ReviewerId,
                ProductId: pr.ProductId
            ))
            .ToList() ?? new List<ProductReviewsDto>(),
                IsInWishlist: false
            );
        }
    }
}
