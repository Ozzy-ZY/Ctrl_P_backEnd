using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public record ProductDTO(
        int Id,
        string Name,
        string Description,
        decimal Price,
        decimal? OldPrice,
        int UnitsInStock,
        List<int>? ProductCategories,
        List<int>? ProductFrames,
        List<int>? ProductMaterials,
        List<int>? ProductSizes,
        List<string>? CategoryNames,
        List<string>? FramesNames,
        List<string>? MaterialsNames,
        List<string>? SizesNames,
        List<string>? Url, // Add this line
        List<IFormFile> Image
    );
}
