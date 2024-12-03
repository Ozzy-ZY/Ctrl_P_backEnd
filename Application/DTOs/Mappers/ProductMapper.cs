using Application.DTOs.CategorizingModels;
using Domain.Models;
using Domain.Models.ProductModels;
using System;
using System.Net.NetworkInformation;

namespace Application.DTOs.Mappers
{
    public static class ProductMapper
    {
        public static Product DtoAsProductCreate(this ProductDTO dto, string url)
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
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ProductCategories = dto.ProductCategories?.Select(id => new ProductCategory
                {
                    CategoryId = id
                }).ToList() ?? new List<ProductCategory>(),
                ProductFrames = dto.ProductFrames?.Select(id => new ProductFrame
                {
                    FrameId = id
                }).ToList() ?? new List<ProductFrame>(),
                ProductMaterials = dto.ProductMaterials?.Select(id => new ProductMaterial
                {
                    MaterialId = id
                }).ToList() ?? new List<ProductMaterial>(),
                ProductSizes = dto.ProductSizes?.Select(id => new ProductSize
                {
                    SizeId = id
                }).ToList() ?? new List<ProductSize>(),
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
                ProductCategories = (dto.ProductCategories ?? Enumerable.Empty<int>()).Select(id => new ProductCategory
                {
                    CategoryId = id
                }).ToList(),
                ProductFrames = (dto.ProductFrames ?? Enumerable.Empty<int>()).Select(id => new ProductFrame
                {
                    FrameId = id
                }).ToList(),
                ProductMaterials = (dto.ProductMaterials ?? Enumerable.Empty<int>()).Select(id => new ProductMaterial
                {
                    MaterialId = id
                }).ToList(),
                ProductSizes = (dto.ProductSizes ?? Enumerable.Empty<int>()).Select(id => new ProductSize
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
                ProductCategories: product.ProductCategories?
                    .Where(pc => pc.CategoryId != 0)
                    .Select(pc => pc.CategoryId)
                    .ToList(),
                CategoryNames: product.ProductCategories?
                    .Select(pc => pc.Category?.Name)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Cast<string>()
                    .ToList(),
                ProductFrames: product.ProductFrames?
                    .Where(pf => pf.FrameId != 0)
                    .Select(pf => pf.FrameId)
                    .ToList(),
                FramesNames: product.ProductFrames?
                    .Select(pf => pf.Frame?.Name)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Cast<string>()
                    .ToList(),
                ProductMaterials: product.ProductMaterials?
                    .Where(pm => pm.MaterialId != 0)
                    .Select(pm => pm.MaterialId)
                    .ToList(),
                MaterialsNames: product.ProductMaterials?
                    .Select(pm => pm.Material?.Name)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Cast<string>()
                    .ToList(),
                ProductSizes: product.ProductSizes?
                    .Where(ps => ps.SizeId != 0)
                    .Select(ps => ps.SizeId)
                    .ToList(),
                SizesNames: product.ProductSizes?
                    .Select(ps => ps.Size?.Name)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Cast<string>()
                    .ToList(),
                Url: product.ProductPhotos?
                    .Select(pp => pp.Url)
                    .ToList(),
                Image: null! // Assuming images are not stored in the Product entity

            );
        }
    }
}
