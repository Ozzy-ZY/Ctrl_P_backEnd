using Application.DTOs.CategorizingModels;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost("add")]
    [Authorize]

    public async Task<IActionResult> AddCategory([FromForm] CategoryDto categoryDto)
    {
        return Ok(await _categoryService.CreateCategoryAsync(categoryDto));
    }

    [HttpPut("update")]
    [Authorize]

    public async Task<IActionResult> UpdateCategory([FromForm] CategoryDto categoryDto)

    {
        return Ok(await _categoryService.UpdateCategoryAsync(categoryDto));
    }

    [HttpGet("Get-All")]
    public async Task<IActionResult> GetAllCategories()
    {
        return Ok(await _categoryService.GetAllCategoriesAsync());
    }

    [HttpDelete("DeleteCategory/{CategoryId}")]
    [Authorize]

    public async Task<IActionResult> DeleteCategory(int CategoryId)

    {
        return Ok(await _categoryService.DeleteCategoryAsync(CategoryId));
    }
}
