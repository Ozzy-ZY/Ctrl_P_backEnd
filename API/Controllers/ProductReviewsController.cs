using System.Security.Claims;
using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductReviewsController : ControllerBase
    {
        private readonly ProductReviewsService _productReviewService;

        public ProductReviewsController(ProductReviewsService productReviewService)
        {
            _productReviewService = productReviewService;
        }
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddReview([FromBody] ProductReviewsDto review)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            review = review with { ReviewerId = userId };
            return Ok(await _productReviewService.CreateProductReviewAsync(review));
        }
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateReview([FromBody] ProductReviewsDto review)
        {
            return Ok(await _productReviewService.UpdateProductReviewAsync(review));
        }
        [HttpDelete("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteReview([FromBody] ProductReviewsDto review)
        {
            return Ok(await _productReviewService.DeleteProductReviewAsync(review));
        }
    }
}
