using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Mappers
{
    public static class ProductMapper
    {
        public static Product DtoAsProductCreate(this ProductDTO dto)
        {
            return new Product()
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                InStock = dto.UnitsInStock > 0 ? true : false,
                InStockAmount = dto.UnitsInStock,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl,
            };
        }

        public static Product DtoAsProductUpdate(this ProductDTO dto)
        {
            return new Product()
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                InStock = dto.UnitsInStock > 0 ? true : false,
                InStockAmount = dto.UnitsInStock,
                UpdatedAt = DateTime.Now,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl,
            };
        }

        public static ProductDTO ProductAsDto(this Product product)
        {
            return new ProductDTO(
                Id: product.Id,
                Name: product.Name,
                Description: product.Description,
                UnitsInStock: product.InStockAmount,
                Category: product.Category,
                ImageUrl: product.ImageUrl
                );
        }
    }
}