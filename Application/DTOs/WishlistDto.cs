using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record WishlistDto
    (
        int UserId,
        int ProductId,
        string? Name,
        string? Description,
        decimal? Price,
        int? UnitsInStock,
        decimal? Rating,
        decimal? OldPrice,
        List<int>? ProductFrameIds,
        List<string>? SizeNames,
        List<string>? Url,
        bool? IsInWishlist
    );
}
