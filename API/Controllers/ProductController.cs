using Application.DTOs;
using Application.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

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
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO productDTO)
        {
            ValidationResult result = await _validator.ValidateAsync(productDTO);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            return Ok(await _productService.CreateProductAsync(productDTO));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProducts()
        {
            return Ok(await _productService.GetAllProductsAsync());
        }
    }
}