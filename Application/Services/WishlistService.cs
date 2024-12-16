using Application.DTOs;
using Application.DTOs.Mappers;
using Domain.Models;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Application.Services;
public class WishlistService
{
    private readonly IUnitOfWork _unitOfWork;

    public WishlistService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult> AddToWishlistAsync(WishlistDto wishlistDto)
    {
        ServiceResult result = new ServiceResult();
        var wishlist = wishlistDto.ToWishlist();
        await _unitOfWork.Wishlists.AddAsync(wishlist);
        var product = await _unitOfWork.Products.GetAsync(product => product.Id == wishlist.ProductId);
        product!.RowVersion++;
        await _unitOfWork.Products.UpdateAsync(product);
        if (await _unitOfWork.CommitAsync() > 0)
        {
            result.Success = true;
            return result;
        }

        result.Errors.Add("Couldn't add the product to Your Wishlist");
        result.Success = false;
        return result;
    }

    public async Task<ServiceResult> RemoveFromWishlistAsync(WishlistDto wishlistDto)
    {
        ServiceResult result = new ServiceResult();
        await _unitOfWork.Wishlists.DeleteAsync(wishlistDto.ToWishlist());
        var product = await _unitOfWork.Products.GetAsync(product => product.Id == wishlistDto.ProductId);
        product!.RowVersion++;
        await _unitOfWork.Products.UpdateAsync(product);
        if (await _unitOfWork.CommitAsync() > 0)
        {
            result.Success = true;
            return result;
        }

        result.Errors.Add("Couldn't add the product to Your Wishlist");
        result.Success = false;
        return result;
    }

    public async Task<List<WishlistDto>> GetAllWishlistAsync(int userId)
    {
        var wishlists = await _unitOfWork.Wishlists.GetAllAsync(
            w => w.UserId == userId,
            q => q.Include(w => w.Product)
                  .ThenInclude(p => p.ProductPhotos.OrderBy(photo => photo.Id).Take(1))
                  .Include(w => w.Product.ProductFrames)
                  .ThenInclude(pf => pf.Frame)
        );

        var wishlistDtos = wishlists.Select(p =>
        {
            var wishlistDto = p.ToDTO();
            wishlistDto = wishlistDto with { IsInWishlist = IsProductInWishlistAsync(userId, wishlistDto.ProductId).Result };
            return wishlistDto;
        }).ToList();

        return wishlistDtos;
    }

    public async Task<bool> IsProductInWishlistAsync(int userId, int productId)
    {
        var wishlistItem = await _unitOfWork.Wishlists.GetAllAsync(w => w.UserId == userId && w.ProductId == productId);

        return wishlistItem.Any();
    }
}
