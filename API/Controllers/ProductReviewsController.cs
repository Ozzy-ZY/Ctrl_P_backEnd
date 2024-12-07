using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<string>> AddReview([FromBody] ProductReviewsDto review)
        {
            return (await _productReviewService.CreateProductReviewAsync(review)).ToString();
        }
        [HttpPut("update")]
        [Authorize]
        public async Task<ActionResult<string>> UpdateReview([FromBody] ProductReviewsDto review)
        {
            return (await _productReviewService.UpdateProductReviewAsync(review)).ToString();
        }
        [HttpDelete("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteReview([FromBody] ProductReviewsDto review)
        {
            return Ok(await _productReviewService.DeleteProductReviewAsync(review));
        }
    }
}
