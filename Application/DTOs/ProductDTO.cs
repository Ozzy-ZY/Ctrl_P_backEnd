using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public record ProductDTO(
        int Id,
        string? Name,
        string? Description,
        decimal Price,
        decimal? OldPrice,
        int UnitsInStock,
        decimal? Rating,
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
        List<ProductReviewsDto>? Reviews,
        bool? IsInWishlist
    );
    public record ProductDTOCreate(
        int Id,
        string Name,
        string Description,
        decimal Price,
        decimal? OldPrice,
        int UnitsInStock,
        List<string>? CategoryNames,
        List<string>? FramesNames,
        List<string>? MaterialsNames,
        List<string>? SizesNames,
        List<IFormFile> Image
    );
    public record ProductDtoDelete(
        int Id
    );
}
