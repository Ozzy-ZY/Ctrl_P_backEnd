using Domain.Models;

namespace Application.DTOs.Mappers;

public static class CartMapper
{
    public static CartDTO ToCartDTO(this Cart cart)
    {
        return new CartDTO(
            UserId: cart.UserId,
            CartId: cart.Id,
            TotalPrice: cart.TotalPrice,
            CreatedAt: cart.CreatedAt,
            IsActive:cart.IsActive
            );
    }

    public static Cart ToCartCreate(this CartDTO cartDTO)
    {
        return new Cart()
        {
            UserId = cartDTO.UserId,
            TotalPrice = cartDTO.TotalPrice,
            CreatedAt = cartDTO.CreatedAt,
            IsActive = cartDTO.IsActive
        };
    }
    public static Cart ToCartUpdate(this CartDTO cartDTO)
    {
        return new Cart()
        {
            UserId = cartDTO.UserId,
            TotalPrice = cartDTO.TotalPrice,
            CreatedAt = cartDTO.CreatedAt,
            UpdatedAt = DateTime.Now,
            IsActive = cartDTO.IsActive
        };
    }}