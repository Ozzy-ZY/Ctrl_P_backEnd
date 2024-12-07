using Application.DTOs;
using Application.DTOs.Mappers;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
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
            // Fetch the existing review
            var existingReview = await _unitOfWork.ProductReviews.GetAsync(p => p.Id == productReviewDto.Id);


            // Check if the reviewer IDs match
            if (existingReview.ReviewerId != productReviewDto.ReviewerId)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this review.");
            }
            existingReview.Review = productReviewDto.Review;
            existingReview.Rating = productReviewDto.Rating;
            existingReview.ReviewDate = DateTime.Now;
            existingReview.Name = productReviewDto.ReviewerName;
            existingReview.Email = productReviewDto.ReviewerEmail;
            return await _unitOfWork.CommitAsync();
        }

        public async Task<int> DeleteProductReviewAsync(ProductReviewsDto productReviewDto)
        {
            // Fetch the existing review from the database
            var existingReview = await _unitOfWork.ProductReviews.GetAsync(p=>p.Id==productReviewDto.Id);

            // Check if the reviewer IDs match
            if (existingReview.ReviewerId != productReviewDto.ReviewerId)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this review.");
            }

            await _unitOfWork.ProductReviews.DeleteAsync(existingReview);
            return await _unitOfWork.CommitAsync();
        }
    }
}
