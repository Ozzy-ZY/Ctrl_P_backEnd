using Application.DTOs;
using Application.DTOs.Mappers;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateProductAsync(ProductDTO dto)
        {
            var product = dto.DtoAsProductCreate();
            await _unitOfWork.Products.AddAsync(product);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var prods = await _unitOfWork.Products.GetAllAsync();
            var productsDto = new List<ProductDTO>();
            foreach (var product in prods)
            {
                productsDto.Add(product.ProductAsDto());
            }
            return productsDto;
        }
    }
}