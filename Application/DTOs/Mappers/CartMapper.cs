using Domain.Models;

namespace Application.DTOs.Mappers;

public static class CartMapper
{
    public static CartDTO ToCartDTO(this Cart cart)
    {
        return new CartDTO(
            UserId: cart.UserId,
            CartId: cart.Id,
            CartItems: cart.CartItems,
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
            CartItems = cartDTO.CartItems,
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
    }

    public static CartItem ToCartItem(this AddToCartDTO addToCartDTO, int cartId)
    {
        return new CartItem()
        {
            ProductId = addToCartDTO.ProductId,
            Quantity = addToCartDTO.Quantity,
            AddedAt = DateTime.Now,
            CartId = cartId
        };
    }
}