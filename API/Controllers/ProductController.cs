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
        private readonly IValidator<ProductDTOCreate> _validatorCreate;

        public ProductController(IProductService productService, IValidator<ProductDTO> validator, IValidator<ProductDTOCreate> validatorCreate)
        {
            _productService = productService;
            _validator = validator;
            _validatorCreate = validatorCreate;
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

        [HttpGet("GetAllProducts/{userId?}")]
        public async Task<IActionResult> GetAllProducts(int? userId)
        {
            return Ok(await _productService.GetAllProductsAsync(userId));
        }
        [HttpGet("GetProduct/{Id}/{userId?}")]
        public async Task<IActionResult> GetProduct(int Id, int? userId)
        {
            return Ok(await _productService.GetProductAsync(Id,userId));
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