﻿using Application.DTOs;
using Application.DTOs.CategorizingModels;
using Application.Services;
using FluentValidation;
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
    public async Task<ActionResult<string>> AddCategory([FromBody] CategoryDTO categoryDto)
    {
        return (await _categoryService.CreateCategoryAsync(categoryDto)).ToString();
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<ActionResult<string>> UpdateCategory([FromBody] CategoryDTO categoryDto)
    {
        return (await _categoryService.UpdateCategoryAsync(categoryDto)).ToString();
    }

    [HttpGet("Get-All")]
    public async Task<IActionResult> GetAllCategories()
    {
        return Ok(await _categoryService.GetAllCategoriesAsync());
    }

    [HttpDelete("Delete")]
    [Authorize]
    public async Task<IActionResult> DeleteCategory([FromBody] CategoryDTO categoryDto)
    {
        return Ok(await _categoryService.DeleteCategoryAsync(categoryDto));
    }
}
