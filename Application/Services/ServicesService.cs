using Application.DTOs;
using Application.DTOs.Mappers;
using Domain.Models;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;

namespace Application.Services;

public class ServicesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;

    public ServicesService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
    {
        _environment = environment;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> AddService(ServiceDTO serviceDTO)
    {
        string uploadsFolder = Path.Combine(_environment.WebRootPath, "Services");
        Directory.CreateDirectory(uploadsFolder);
        string uniqueFilename = Guid.NewGuid().ToString() + Path.GetExtension(serviceDTO.Image.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFilename);
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await serviceDTO.Image.CopyToAsync(fileStream);
        Service service = serviceDTO.ToService($"/Services/{uniqueFilename}");
        await _unitOfWork.Services.AddAsync(service);
        return await _unitOfWork.CommitAsync();
    }
}