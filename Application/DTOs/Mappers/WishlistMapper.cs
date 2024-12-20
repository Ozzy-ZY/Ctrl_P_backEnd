using Domain.Models.ShopModels;

namespace Application.DTOs.Mappers;

    public static class WishlistMapper
{
    public static WishList ToWishlist(this WishlistDto wishlistDto)
    {
        return new WishList
        {
            UserId = wishlistDto.UserId,
            ProductId = wishlistDto.ProductId,
            AddedAt = DateTime.Now
        };
    }
    public static WishlistDto ToDTO(this WishList wishlist)
    {
        if (wishlist == null) throw new ArgumentNullException(nameof(wishlist));
        if (wishlist.Product == null) throw new ArgumentNullException(nameof(wishlist.Product));

        return new WishlistDto(
            wishlist.UserId,
            wishlist.ProductId,
            wishlist.Product.Name ?? string.Empty, // Handle potential null
            wishlist.Product.Description ?? string.Empty, // Handle potential null
            wishlist.Product.Price,
            wishlist.Product.InStockAmount,
            wishlist.Product.Rating,
            wishlist.Product.OldPrice,
            wishlist.Product.ProductSizes?.Select(pf => pf.SizeId).ToList() ?? new List<int>(), // Null safety
            wishlist.Product.ProductSizes?.Select(pf => pf.Size?.Name ?? string.Empty).ToList() ?? new List<string>(), // Null safety
            wishlist.Product.ProductPhotos?.OrderBy(photo => photo.Id).Take(1).Select(photo => photo.Url ?? string.Empty).ToList() ?? new List<string>(), // Null safety
            IsInWishlist: false
        );
    }

}

