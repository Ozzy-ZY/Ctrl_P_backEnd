using System.Security.Cryptography;
using Application.DTOs;
using Application.DTOs.CategorizingModels;
using Application.DTOs.Mappers.CategorizingModels;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class CategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;

    public CategoryService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
    {
        _unitOfWork = unitOfWork;
        _environment = environment;
    }

    public async Task<ServiceResult> CreateCategoryAsync(CategoryDto categoryDto)
    {
        var result = new ServiceResult();

        if (categoryDto.Image == null)
        {
            result.Errors.Add("Category image is required.");
            result.Success = false;
            return result;
        }

        var (filename, hash) = await GenerateImageDetailsAsync(categoryDto.Image, "Categories");
        var category = categoryDto.ToCategory($"/Categories/{filename}");
        category.Hash = hash;

        await _unitOfWork.Categories.AddAsync(category);
        var commitResult = await _unitOfWork.CommitAsync();

        if (commitResult > 0)
        {
            var saveImageResult = await SaveImageToFileSystemAsync(categoryDto.Image, "Categories", filename);
            if (!saveImageResult.Success)
            {
                result.Errors.Add($"Category created, but image saving failed: {string.Join(", ", saveImageResult.Errors)}");
                result.Success = false;
                return result;
            }

            result.Success = true;
            return result;
        }

        result.Errors.Add("Failed to create category in the database.");
        result.Success = false;
        return result;
    }

    public async Task<ServiceResult> UpdateCategoryAsync(CategoryDto categoryDto)
    {
        var result = new ServiceResult();

        var existingCategory = await _unitOfWork.Categories.GetAsync(c => c.Id == categoryDto.Id);
        if (existingCategory == null)
        {
            result.Errors.Add($"Category with ID {categoryDto.Id} not found.");
            result.Success = false;
            return result;
        }

        if (categoryDto.Image != null)
        {
            var incomingHash = ComputeHash(categoryDto.Image);
            if (existingCategory.Hash != incomingHash)
            {
                var deleteImageResult = await DeleteImageAsync(existingCategory.ImageUrl);
                if (!deleteImageResult.Success)
                {
                    result.Errors.AddRange(deleteImageResult.Errors);
                    result.Success = false;
                    return result;
                }

                var uniqueFilename = Guid.NewGuid() + Path.GetExtension(categoryDto.Image.FileName);
                existingCategory.ImageUrl = $"/Categories/{uniqueFilename}";
                existingCategory.Hash = incomingHash;

                var saveImageResult = await SaveImageToFileSystemAsync(categoryDto.Image, "Categories", uniqueFilename);
                if (!saveImageResult.Success)
                {
                    result.Errors.Add($"Image update failed: {string.Join(", ", saveImageResult.Errors)}");
                    result.Success = false;
                    return result;
                }
            }
        }

        existingCategory.Name = categoryDto.Name;

        await _unitOfWork.Categories.UpdateAsync(existingCategory);
        var commitResult = await _unitOfWork.CommitAsync();

        result.Success = commitResult > 0;
        if (!result.Success)
        {
            result.Errors.Add("Failed to update category in the database.");
        }

        return result;
    }

    public async Task<ServiceResult> DeleteCategoryAsync(int categoryId)
    {
        var result = new ServiceResult();

        var existingCategory = await _unitOfWork.Categories.GetAsync(c => c.Id == categoryId);
        if (existingCategory == null)
        {
            result.Errors.Add($"Category with ID {categoryId} not found.");
            result.Success = false;
            return result;
        }

        var deleteImageResult = await DeleteImageAsync(existingCategory.ImageUrl);
        if (!deleteImageResult.Success)
        {
            result.Errors.AddRange(deleteImageResult.Errors);
            result.Success = false;
            return result;
        }

        await _unitOfWork.Categories.DeleteAsync(existingCategory);
        var commitResult = await _unitOfWork.CommitAsync();

        result.Success = commitResult > 0;
        if (!result.Success)
        {
            result.Errors.Add("Failed to delete category from the database.");
        }

        return result;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return categories.Select(c => c.ToDTO());
    }

    private async Task<(string uniqueFilename, string imageHash)> GenerateImageDetailsAsync(IFormFile image, string folderName)
    {
        string uniqueFilename = Guid.NewGuid() + Path.GetExtension(image.FileName);
        string imageHash = ComputeHash(image);

        return await Task.FromResult((uniqueFilename, imageHash));
    }

    private async Task<ServiceResult> SaveImageToFileSystemAsync(IFormFile image, string folderName, string uniqueFilename)
    {
        var result = new ServiceResult();

        if (image == null || image.Length == 0)
        {
            result.Errors.Add("Invalid image file.");
            result.Success = false;
            return result;
        }

        string uploadsFolder = Path.Combine(_environment.WebRootPath, folderName);
        Directory.CreateDirectory(uploadsFolder);

        string filePath = Path.Combine(uploadsFolder, uniqueFilename);

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        result.Success = true;
        return result;
    }

    private async Task<ServiceResult> DeleteImageAsync(string imageUrl)
    {
        var result = new ServiceResult();

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            result.Success = true;
            return result;
        }

        string filePath = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('/'));
        if (File.Exists(filePath))
        {
            await Task.Run(() => File.Delete(filePath));
            result.Success = true;
        }
        else
        {
            result.Errors.Add($"File not found: {imageUrl}");
            result.Success = false;
        }

        return result;
    }

    private string ComputeHash(IFormFile image)
    {
        using var md5 = MD5.Create();
        using var stream = image.OpenReadStream();
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
