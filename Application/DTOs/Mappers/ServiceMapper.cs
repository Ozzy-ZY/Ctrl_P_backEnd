using Domain.Models;

namespace Application.DTOs.Mappers;

public static class ServiceMapper
{
    public static Service ToService(this ServiceDTO serviceDTO, string url)
    {
        return new Service
        {
            Id = serviceDTO.Id,
            Name = serviceDTO.Name,
            Description = serviceDTO.Description,
            ImageUrl = url
        };
    }

    public static ServiceDTO ToDto(this Service service)
    {
        return new ServiceDTO(Id: service.Id, Name: service.Name, Description: service.Description, Image: null!, ImageUrl: service.ImageUrl);
    }
}