using Application.DTOs;

namespace Application.Services
{
    public interface IProductService
    {
        public Task<int> CreateProductAsync(ProductDTO dto);

        public Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        public Task<int> UpdateProductAsync(ProductDTO dto);
        public Task<int> DeleteProductAsync(ProductDTO dto);
    }
}