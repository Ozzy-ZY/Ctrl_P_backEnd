using Application.DTOs;
using Application.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IValidator<ProductDTO> _validator;

        public ProductController(IProductService productService, IValidator<ProductDTO> validator)
        {
            _productService = productService;
            _validator = validator;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDTO productDto)
        {
            ValidationResult result = await _validator.ValidateAsync(productDto);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            return Ok(await _productService.CreateProductAsync(productDto));
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            return Ok(await _productService.GetAllProductsAsync());
        }
        [HttpGet("GetProduct/{Id}")]
        public async Task<IActionResult> GetProduct(int Id)
        {
            return Ok(await _productService.GetProductAsync(Id));
        }
        [HttpPut("UpdateProduct")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductDTO productDto)
        {
            ValidationResult result = await _validator.ValidateAsync(productDto);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            return Ok(await _productService.UpdateProductAsync(productDto));
        }

        [HttpDelete("DeleteProduct")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct([FromForm] ProductDTO productDto)
        {
            return Ok(await _productService.DeleteProductAsync(productDto));
        }
    }
}