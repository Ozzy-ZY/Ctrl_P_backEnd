using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public record ProductDTO(
        int Id,
        string Name,
        string? Description,
        decimal Price,
        decimal? OldPrice,
        int UnitsInStock,
        List<int>? ProductCategoryIds,
        List<string>? CategoryNames,
        List<int>? ProductFrameIds,
        List<string>? FramesNames,
        List<int>? ProductMaterialIds,
        List<string>? MaterialsNames,
        List<int>? ProductSizeIds,
        List<string>? SizesNames,
        List<string>? Url,
        List<IFormFile>? Image,
        List<string>? Review,
        List<decimal>? Rating,
        List<string>? ReviewerName,
        List<DateTime>? ReviewDate,
        bool? IsInWishlist
    );
}
