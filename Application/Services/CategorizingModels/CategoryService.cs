using Application.DTOs;
using Application.DTOs.CategorizingModels;
using Application.DTOs.Mappers.CategorizingModels;
using Domain.Models.CategorizingModels;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;

namespace Application.Services;

public class CategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;

    public CategoryService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
    {
        _environment = environment;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateCategoryAsync(CategoryDto categoryDTO)
    {
        var category = categoryDTO.ToCategory();
        await _unitOfWork.Categories.AddAsync(category);
        return await _unitOfWork.CommitAsync();
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return categories.Select(c => c.ToDTO());
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetAsync(c => c.Id == id);
        return category?.ToDTO();
    }

    public async Task<int> UpdateCategoryAsync(CategoryDto categoryDTO)
    {
        var existingCategory = await _unitOfWork.Categories.GetAsync(c => c.Id == categoryDTO.Id);
        if (existingCategory == null) return 0;

        existingCategory.Name = categoryDTO.Name;

        await _unitOfWork.Categories.UpdateAsync(existingCategory);
        return await _unitOfWork.CommitAsync();
    }

    public async Task<int> DeleteCategoryAsync(CategoryDto categoryDTO)
    {
        var category = categoryDTO.ToCategory();

        await _unitOfWork.Categories.DeleteAsync(category);
        return await _unitOfWork.CommitAsync();
    }
}
