using Application.DTOs;
using Application.DTOs.CategorizingModels;
using Application.DTOs.Mappers.CategorizingModels;
using Domain.Models.CategorizingModels;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;

namespace Application.Services;

public class MaterialService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;

    public MaterialService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
    {
        _environment = environment;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult> CreateMaterialAsync(MaterialDto materialDto)
    {
        var material = materialDto.ToMaterial();
        await _unitOfWork.Materials.AddAsync(material);
        var result = await _unitOfWork.CommitAsync();
        return new ServiceResult { Success = result > 0 };
    }

    public async Task<IEnumerable<MaterialDto>> GetAllMaterialsAsync()
    {
        var materials = await _unitOfWork.Materials.GetAllAsync();
        return materials.Select(m => m.ToDTO());
    }

    public async Task<MaterialDto?> GetMaterialByIdAsync(int id)
    {
        var material = await _unitOfWork.Materials.GetAsync(m => m.Id == id);
        return material?.ToDTO();
    }

    public async Task<ServiceResult> UpdateMaterialAsync(MaterialDto materialDto)
    {
        var existingMaterial = await _unitOfWork.Materials.GetAsync(m => m.Id == materialDto.Id);
        if (existingMaterial == null) return new ServiceResult { Success = false, Errors = new List<string> { "Material not found" } };

        existingMaterial.Name = materialDto.Name;

        await _unitOfWork.Materials.UpdateAsync(existingMaterial);
        var result = await _unitOfWork.CommitAsync();
        return new ServiceResult { Success = result > 0 };
    }

    public async Task<ServiceResult> DeleteMaterialAsync(MaterialDto materialDto)
    {
        var material = materialDto.ToMaterial();

        await _unitOfWork.Materials.DeleteAsync(material);
        var result = await _unitOfWork.CommitAsync();
        return new ServiceResult { Success = result > 0 };
    }
}
