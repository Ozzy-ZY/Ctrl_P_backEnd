using Domain.Models;

namespace Application.DTOs;

public record CartDTO(int UserId, int CartId, decimal TotalPrice, DateTime CreatedAt, bool IsActive,ICollection<CartItem> CartItems);