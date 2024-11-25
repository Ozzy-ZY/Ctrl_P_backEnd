using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Application.DTOs
{
    public record ProductDTO(
        int Id,
        string Name,
        string Description,
        decimal Price,
        decimal? OldPrice,
        bool Sale,
        int UnitsInStock);
}
