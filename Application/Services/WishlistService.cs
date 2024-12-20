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

        if (wishlistDto == null)
        {
            result.Errors.Add("Invalid wishlist data.");
            result.Success = false;
            return result;
        }

        var product = await _unitOfWork.Products.GetAsync(product => product.Id == wishlistDto.ProductId);
        if (product == null)
        {
            result.Errors.Add("Product not found.");
            result.Success = false;
            return result;
        }

        var wishlist = wishlistDto.ToWishlist();
        await _unitOfWork.Wishlists.AddAsync(wishlist);

        product.RowVersion++;
        await _unitOfWork.Products.UpdateAsync(product);

        if (await _unitOfWork.CommitAsync() > 0)
        {
            result.Success = true;
            return result;
        }

        result.Errors.Add("Couldn't add the product to your wishlist.");
        result.Success = false;
        return result;
    }

    public async Task<ServiceResult> RemoveFromWishlistAsync(int productId)
    {
        ServiceResult result = new ServiceResult();

        var wishlist = await _unitOfWork.Wishlists.GetAsync(w => w.ProductId == productId);
        if (wishlist == null)
        {
            result.Errors.Add("Wishlist item not found.");
            result.Success = false;
            return result;
        }

        await _unitOfWork.Wishlists.DeleteAsync(wishlist);

        var product = await _unitOfWork.Products.GetAsync(product => product.Id == productId);
        if (product != null)
        {
            product.RowVersion++;
            await _unitOfWork.Products.UpdateAsync(product);
        }

        if (await _unitOfWork.CommitAsync() > 0)
        {
            result.Success = true;
            return result;
        }

        result.Errors.Add("Couldn't remove the product from your wishlist.");
        result.Success = false;
        return result;
    }

    public async Task<List<WishlistDto>> GetAllWishlistAsync(int userId)
    {
        if (userId <= 0)
        {
            return new List<WishlistDto>(); // Return an empty list for invalid user IDs
        }

        var wishlists = await _unitOfWork.Wishlists.GetAllAsync(
            w => w.UserId == userId,
            q => q.Include(w => w.Product)
                  .ThenInclude(p => p.ProductPhotos.OrderBy(photo => photo.Id).Take(1))
                  .Include(w => w.Product.ProductFrames)
                  .ThenInclude(pf => pf.Frame)
        );

        if (wishlists == null || !wishlists.Any())

        {
            return new List<WishlistDto>();
        }

        var wishlistDtos = new List<WishlistDto>();

        foreach (var wishlist in wishlists)
        {
            if (wishlist.Product == null) continue;

            var wishlistDto = wishlist.ToDTO();
            wishlistDto = wishlistDto with { IsInWishlist = await IsProductInWishlistAsync(userId, wishlistDto.ProductId) };
            wishlistDtos.Add(wishlistDto);
        }

        return wishlistDtos;
    }

    public async Task<bool> IsProductInWishlistAsync(int userId, int productId)
    {
        if (userId <= 0 || productId <= 0) return false;

        var wishlistItem = await _unitOfWork.Wishlists.GetAllAsync(w => w.UserId == userId && w.ProductId == productId);
        return wishlistItem.Any();
    }
}
