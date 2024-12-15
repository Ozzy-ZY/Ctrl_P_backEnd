using System.Security.Cryptography;
using Application.DTOs;
using Application.DTOs.CategorizingModels;
using Application.DTOs.Mappers.CategorizingModels;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
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
                    throw new ArgumentException("Category image is required.", nameof(categoryDto.Image));

                var (filename, hash) = await GenerateImageDetailsAsync(categoryDto.Image, "Categories");
                var category = categoryDto.ToCategory($"/Categories/{filename}");
                category.Hash = hash;

                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.CommitAsync();

                // Save the image only if creation was successful
                await SaveImageToFileSystemAsync(categoryDto.Image, "Categories", filename);

                result.Success = true;
            if (!result.Success)
            {
                await DeleteImageAsync($"/Categories/{filename}");
            }


            return result;
        }

        public async Task<ServiceResult> UpdateCategoryAsync(CategoryDto categoryDto)
        {
            var result = new ServiceResult();

                var existingCategory = await _unitOfWork.Categories.GetAsync(c => c.Id == categoryDto.Id)
                    ?? throw new KeyNotFoundException($"Category with ID {categoryDto.Id} not found.");

                if (categoryDto.Image != null)
                {
                    var incomingHash = ComputeHash(categoryDto.Image);
                    if (existingCategory.Hash != incomingHash)
                    {
                        // Image has changed, delete the old one and save the new one
                        await DeleteImageAsync(existingCategory.ImageUrl);

                        var uniqueFilename = Guid.NewGuid() + Path.GetExtension(categoryDto.Image.FileName);
                        existingCategory.ImageUrl = $"/Categories/{uniqueFilename}";
                        existingCategory.Hash = incomingHash;

                        await SaveImageToFileSystemAsync(categoryDto.Image, "Categories", uniqueFilename);
                    }
                }

                existingCategory.Name = categoryDto.Name;

                await _unitOfWork.Categories.UpdateAsync(existingCategory);
                await _unitOfWork.CommitAsync();

                result.Success = true;

            return result;
        }

        public async Task<ServiceResult> DeleteCategoryAsync(CategoryDto categoryDto)
        {
            var result = new ServiceResult();

                var existingCategory = await _unitOfWork.Categories.GetAsync(c => c.Id == categoryDto.Id)
                    ?? throw new KeyNotFoundException($"Category with ID {categoryDto.Id} not found.");

                await DeleteImageAsync(existingCategory.ImageUrl);
                await _unitOfWork.Categories.DeleteAsync(existingCategory);
                await _unitOfWork.CommitAsync();

                result.Success = true;
            return result;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return categories.Select(c => c.ToDTO());
        }

        private async Task<(string uniqueFilename, string imageHash)> GenerateImageDetailsAsync(IFormFile image, string folderName)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Invalid image file.", nameof(image));

            string uniqueFilename = Guid.NewGuid() + Path.GetExtension(image.FileName);
            string imageHash = ComputeHash(image);

            return await Task.FromResult((uniqueFilename, imageHash));
        }

        private async Task SaveImageToFileSystemAsync(IFormFile image, string folderName, string uniqueFilename)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath, folderName);
            Directory.CreateDirectory(uploadsFolder);

            string filePath = Path.Combine(uploadsFolder, uniqueFilename);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
        }

        private async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;

            string filePath = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        private string ComputeHash(IFormFile image)
        {
            using var md5 = MD5.Create();
            using var stream = image.OpenReadStream();
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
