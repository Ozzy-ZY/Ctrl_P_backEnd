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
    public class ProductReviewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductReviewsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> CreateProductReviewAsync(ProductReviewsDto productReviewDto)
        {
            await _unitOfWork.ProductReviews.AddAsync(productReviewDto.ToProductReview());
            return await _unitOfWork.CommitAsync();
        }
        public async Task<int> UpdateProductReviewAsync(ProductReviewsDto productReviewDto)
        {
            await _unitOfWork.ProductReviews.UpdateAsync(productReviewDto.ToProductReview());
            return await _unitOfWork.CommitAsync();
        }
        public async Task<int> DeleteProductReviewAsync(ProductReviewsDto productReviewDto)
        {
            await _unitOfWork.ProductReviews.DeleteAsync(productReviewDto.ToProductReview());
            return await _unitOfWork.CommitAsync();
        }
    }
}
