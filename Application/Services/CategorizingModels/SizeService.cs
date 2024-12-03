using Application.DTOs.CategorizingModels;
using Application.DTOs.Mappers.CategorizingModels;
using Domain.Models.CategorizingModels;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.CategorizingModels;

public class SizeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;

    public SizeService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
    {
        _environment = environment;
        _unitOfWork = unitOfWork;
    }
    public async Task<int> CreateSizeAsync(SizeDto sizeDto)
    {
        var size = sizeDto.ToSize();
        await _unitOfWork.Sizes.AddAsync(size);
        return await _unitOfWork.CommitAsync();
    }
    public async Task<IEnumerable<SizeDto>> GetAllSizesAsync()
    {
        var sizes = await _unitOfWork.Sizes.GetAllAsync();
        return sizes.Select(s => s.ToDTO());
    }
    public async Task<SizeDto?> GetSizeByIdAsync(int id)
    {
        var size = await _unitOfWork.Sizes.GetAsync(s => s.Id == id);
        return size?.ToDTO();
    }
    public async Task<int> UpdateSizeAsync(SizeDto sizeDto)
    {
        var existingSize = await _unitOfWork.Sizes.GetAsync(s => s.Id == sizeDto.Id);
        if (existingSize == null) return 0;
        existingSize.Name = sizeDto.Name;
        await _unitOfWork.Sizes.UpdateAsync(existingSize);
        return await _unitOfWork.CommitAsync();
    }
    public async Task<int> DeleteSizeAsync(SizeDto sizeDto)
    {
        var size = sizeDto.ToSize();
        await _unitOfWork.Sizes.DeleteAsync(size);
        return await _unitOfWork.CommitAsync();
    }
}

