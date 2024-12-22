using Application.DTOs;
using Application.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IValidator<ProductDTO> _validator;
        private readonly IValidator<ProductDTOCreate> _validatorCreate;
        private readonly IValidator<ProductDTOUpdate> _validatorUpdate;

        public ProductController(IProductService productService, IValidator<ProductDTO> validator, IValidator<ProductDTOCreate> validatorCreate, IValidator<ProductDTOUpdate> validatorUpdate)
        {
            _productService = productService;
            _validator = validator;
            _validatorCreate = validatorCreate;
            _validatorUpdate = validatorUpdate;
            _validatorUpdate = validatorUpdate;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDTOCreate productDto)
        {
            ValidationResult result = await _validatorCreate.ValidateAsync(productDto);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            return Ok(await _productService.CreateProductAsync(productDto));
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            int? userId = null;

            // Check if the user is authenticated and extract userId from the token
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                }
            }
            return Ok(await _productService.GetAllProductsAsync(userId));
        }
        [HttpGet("GetProduct/{Id}")]
        public async Task<IActionResult> GetProduct(int Id)
        {
            int? userId = null;

            // Check if the user is authenticated and extract userId from the token
            if (User.Identity?.IsAuthenticated == true)
            {
                userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            }
            return Ok(await _productService.GetProductAsync(Id,userId));
        }
        [HttpPut("UpdateProduct")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductDTOUpdate productDto)
        {
            ValidationResult result = await _validatorUpdate.ValidateAsync(productDto);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            return Ok(await _productService.UpdateProductAsync(productDto));
        }

        [HttpDelete("DeleteProduct/{ProductId}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int ProductId)
        {
            return Ok(await _productService.DeleteProductAsync(ProductId));
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterProducts(
            [FromQuery] IEnumerable<int>? categoryIds,
            [FromQuery] IEnumerable<int>? frameIds,
            [FromQuery] IEnumerable<int>? materialIds,
            [FromQuery] IEnumerable<int>? sizeIds,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int? minRating,
            [FromQuery] int? maxRating,
            [FromQuery] string? nameContains,
            [FromQuery] int? userId)
        {

            return Ok(await _productService.FilterProductsAsync(categoryIds, frameIds, materialIds, sizeIds,minPrice, maxPrice, minRating, maxRating, nameContains, userId));
        }

    }
}