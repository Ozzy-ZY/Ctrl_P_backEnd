namespace Application.DTOs
{
    public record ProductDTO(
        int Id,
        string Name,
        string Description,
        int UnitsInStock,
        string Category,
        string ImageUrl);
}