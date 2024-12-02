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

    public async Task<int> UpdateServiceAsync(ServiceDTO serviceDTO)
    {
        // Retrieve the existing service entity from the database by Id
        var existingService = await _unitOfWork.Services.GetAsync(s => s.Id == serviceDTO.Id);

        // Update the image if a new one is provided
        if (serviceDTO.Image != null)
        {
            // Define the upload folder
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "Services");
            Directory.CreateDirectory(uploadsFolder);
            // Delete the old image if it exists
            if (!string.IsNullOrEmpty(existingService.ImageUrl))
            {
                var oldImagePath = Path.Combine(_environment.WebRootPath, existingService.ImageUrl.TrimStart('/'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }
            string uniqueFilename = Guid.NewGuid().ToString() + Path.GetExtension(serviceDTO.Image.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFilename);

            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await serviceDTO.Image.CopyToAsync(fileStream);

            existingService.ImageUrl = $"/Services/{uniqueFilename}";
        }

        // Update other properties
        existingService.Name = serviceDTO.Name;
        existingService.Description = serviceDTO.Description;

        // Use the Unit of Work's Update method if necessary or skip it if tracking is automatic
        await _unitOfWork.Services.UpdateAsync(existingService);

        // Commit the changes
        return await _unitOfWork.CommitAsync();
    }

    public async Task<IEnumerable<ServiceDTO>> GetAllServicesAsync()
    {
        var listOfServices = await _unitOfWork.Services.GetAllAsync();
        List<ServiceDTO> result = new List<ServiceDTO>();

        foreach (var Service in listOfServices)
        {
            result.Add(Service.ToDto());
        }
        return result;
    }
    public async Task<int> DeleteServiceAsync(ServiceDTO serviceDTO)
    {
        var existingService = await _unitOfWork.Services.GetAsync(s => s.Id == serviceDTO.Id);

        if (!string.IsNullOrEmpty(existingService.ImageUrl))
        {
            var oldImagePath = Path.Combine(_environment.WebRootPath, existingService.ImageUrl.TrimStart('/'));
            if (File.Exists(oldImagePath))
            {
                File.Delete(oldImagePath);
            }
        }

        await _unitOfWork.Services.DeleteAsync(existingService);
        return await _unitOfWork.CommitAsync();
    }

}