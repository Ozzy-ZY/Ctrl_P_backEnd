using Domain.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Mappers
{
    public static class ProductReviewsMapper
    {
        public static ProductReviews ToProductReview(this ProductReviewsDto productReviewDto)
        {
            return new ProductReviews
            {
                Id = productReviewDto.Id,
                Review = productReviewDto.Review,
                Rating = productReviewDto.Rating,
                Name = productReviewDto.ReviewerName,
                Email = productReviewDto.ReviewerEmail,
                ReviewerId = productReviewDto.ReviewerId!.Value,
                ProductId = productReviewDto.ProductId,
                ReviewDate = DateTime.Now
            };
        }
        public static ProductReviewsDto ToDTO(this ProductReviews productReview)
        {
            return new ProductReviewsDto
            (
                Id:productReview.Id,
                Review:productReview.Review,
                Rating:productReview.Rating,
                ReviewerName:productReview.Name,
                ReviewerEmail:productReview.Email,
                ReviewerId:productReview.ReviewerId,
                ProductId:productReview.ProductId
            );
        }
    }
}
