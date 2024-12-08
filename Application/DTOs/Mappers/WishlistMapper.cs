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
            return new WishlistDto(
                wishlist.UserId,
                wishlist.ProductId,
                wishlist.Product.Name,
                wishlist.Product.Description,
                wishlist.Product.Price,
                wishlist.Product.InStockAmount,
                wishlist.Product.ProductFrames.Select(pf => pf.FrameId).ToList(),
                wishlist.Product.ProductFrames.Select(pf => pf.Frame.Name).ToList(),
                wishlist.Product.ProductPhotos.OrderBy(photo => photo.Id).Take(1).Select(photo => photo.Url).ToList()
            );
    }
}

