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

        public async Task<ServiceResult> CreateProductReviewAsync(ProductReviewsDto productReviewDto)
        {
            var result = new ServiceResult();
                // Add the new review
                var review = productReviewDto.ToProductReview();
                await _unitOfWork.ProductReviews.AddAsync(review);

                // Commit the transaction to ensure review creation is successful
                var commitResult = await _unitOfWork.CommitAsync();
                if (commitResult <= 0)
                {
                    result.Errors.Add("Failed to create product review.");
                    return result;
                }

                // Update the product rating after successfully adding the review
                var product = await _unitOfWork.Products.GetAsync(p => p.Id == review.ProductId);
                if (product == null)
                {
                    result.Errors.Add("Product not found for updating the rating.");
                    return result;
                }

                product.Rating = await CalculateProductRatingAsync(review.ProductId);
                await _unitOfWork.Products.UpdateAsync(product);

                // Commit the rating update
                await _unitOfWork.CommitAsync();
                result.Success = true;
            

            return result;
        }

        public async Task<ServiceResult> UpdateProductReviewAsync(ProductReviewsDto productReviewDto)
        {
            var result = new ServiceResult();
    
                // Fetch the existing review
                var existingReview = await _unitOfWork.ProductReviews.GetAsync(p => p.Id == productReviewDto.Id);
                if (existingReview == null)
                {
                    result.Errors.Add("Review not found");
                    return result;
                }

                if (existingReview.ReviewerId != productReviewDto.ReviewerId)
                {
                    result.Errors.Add("You do not have permission to update this review.");
                    return result;
                }

                // Update review details
                existingReview.Review = productReviewDto.Review;
                existingReview.Rating = productReviewDto.Rating;
                existingReview.ReviewDate = DateTime.Now;
                existingReview.Name = productReviewDto.ReviewerName;
                existingReview.Email = productReviewDto.ReviewerEmail;

                // Update the product rating
                var product = await _unitOfWork.Products.GetAsync(p => p.Id == existingReview.ProductId);
                if (product == null)
                {
                    result.Errors.Add("Product not found");
                    return result;
                }

                product.Rating = await CalculateProductRatingAsync(existingReview.ProductId);
                await _unitOfWork.Products.UpdateAsync(product);

                await _unitOfWork.CommitAsync();
                result.Success = true;

            return result;
        }

        public async Task<ServiceResult> DeleteProductReviewAsync(ProductReviewsDto productReviewDto)
        {
            var result = new ServiceResult();
                // Fetch the existing review
                var existingReview = await _unitOfWork.ProductReviews.GetAsync(p => p.Id == productReviewDto.Id);
                if (existingReview == null)
                {
                    result.Errors.Add("Review not found");
                    return result;
                }

                if (existingReview.ReviewerId != productReviewDto.ReviewerId)
                {
                    result.Errors.Add("You do not have permission to delete this review.");
                    return result;
                }

                // Delete the review
                await _unitOfWork.ProductReviews.DeleteAsync(existingReview);

                // Update the product rating
                var product = await _unitOfWork.Products.GetAsync(p => p.Id == existingReview.ProductId);
                product.Rating = await CalculateProductRatingAsync(existingReview.ProductId);
                await _unitOfWork.Products.UpdateAsync(product);

                await _unitOfWork.CommitAsync();
                result.Success = true;


            return result;
        }

        private async Task<decimal> CalculateProductRatingAsync(int productId)
        {
            var reviews = await _unitOfWork.ProductReviews.GetAllAsync(pr => pr.ProductId == productId);
            if (reviews.Count() == 0) return 0;

            return reviews.Sum(r => r.Rating) / reviews.Count();
        }
    }
}
