using Application.DTOs;

namespace Application.Services
{
    public interface IProductService
    {
        public Task<ServiceResult> CreateProductAsync(ProductDTOCreate dto);

        public Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int? userId);
        public Task<ProductDTO> GetProductAsync(int Id, int? userId);
        public Task<ServiceResult> UpdateProductAsync(ProductDTO dto);
        public Task<ServiceResult> DeleteProductAsync(ProductDTO dto);
    }
}