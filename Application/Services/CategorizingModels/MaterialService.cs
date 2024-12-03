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

    public async Task<int> CreateMaterialAsync(MaterialDto materialDto)
    {
        var material = materialDto.ToMaterial();
        await _unitOfWork.Materials.AddAsync(material);
        return await _unitOfWork.CommitAsync();
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

    public async Task<int> UpdateMaterialAsync(MaterialDto materialDto)
    {
        var existingMaterial = await _unitOfWork.Materials.GetAsync(m => m.Id == materialDto.Id);
        if (existingMaterial == null) return 0;

        existingMaterial.Name = materialDto.Name;

        await _unitOfWork.Materials.UpdateAsync(existingMaterial);
        return await _unitOfWork.CommitAsync();
    }

    public async Task<int> DeleteMaterialAsync(MaterialDto materialDto)
    {
        var material = materialDto.ToMaterial();

        await _unitOfWork.Materials.DeleteAsync(material);
        return await _unitOfWork.CommitAsync();
    }
}
