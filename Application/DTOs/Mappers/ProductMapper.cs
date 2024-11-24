using Domain.Models;
using System.Net.NetworkInformation;

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
                Price = dto.Price,
                OldPrice = dto.OldPrice,
                Sale = dto.Sale,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
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
                Price = dto.Price,
                OldPrice = dto.OldPrice,
                Sale = dto.Sale,
                UpdatedAt = DateTime.Now
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
                Sale: product.Sale
            );
        }
    }
}
