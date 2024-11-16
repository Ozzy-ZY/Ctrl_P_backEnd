﻿using Domain.Models;

namespace Application.DTOs.Mappers;

public static class ServiceMapper
{
    public static Service ToService(this ServiceDTO serviceDTO, string url)
    {
        return new Service
        {
            Name = serviceDTO.Name,
            Description = serviceDTO.Description,
            ImageUrl = url
        };
    }
}