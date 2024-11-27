namespace Application.DTOs;

public record CartItemDTO(int Id, int CartId, string ProductName, decimal UnitPrice, int Quantity);