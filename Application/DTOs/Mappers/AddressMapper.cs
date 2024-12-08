﻿using Domain.Models;

namespace Application.DTOs.Mappers;

public static class AddressMapper
{
    public static Address AsAddress(this AddressDTO dto, int userId)
    {
        return new Address()
        {
            UserId = userId,
            City = dto.City,
            AddressText = dto.AddressText,
            BillingAddress = dto.BillingAddress,
            Country = dto.Country,
            CompanyName = dto.CompanyName,
            Phone = dto.Phone,
            ZipCode = dto.ZipCode,
            State = dto.State,
            FullName = dto.FullName,
            StreetAddress = dto.StreetAddress,
        };
    }
}