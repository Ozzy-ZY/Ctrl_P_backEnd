﻿using Application.DTOs;

namespace Application.Services
{
    public interface IProductService
    {
        public Task<ServiceResult> CreateProductAsync(ProductDTOCreate dto);

        public Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int? userId);
        public Task<ProductDTO> GetProductAsync(int Id, int? userId);
        public Task<ServiceResult> UpdateProductAsync(ProductDTOCreate dto);
        public Task<ServiceResult> DeleteProductAsync(ProductDtoDelete dto);
        public Task<IEnumerable<ProductDTO>> FilterProductsAsync(
            IEnumerable<int>? categoryIds = null,
            IEnumerable<int>? frameIds = null,
            IEnumerable<int>? materialIds = null,
            IEnumerable<int>? sizeIds = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minRating = null,
            int? maxRating = null,
            string? nameContains = null,
            int? userId = null);
    }
}