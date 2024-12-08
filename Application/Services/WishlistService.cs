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

    public async Task<int> AddToWishlistAsync(WishlistDto wishlistDto)
    {
        var wishlist = wishlistDto.ToWishlist();
        await _unitOfWork.Wishlists.AddAsync(wishlist);
        return await _unitOfWork.CommitAsync();
    }

    public async Task<int> RemoveFromWishlistAsync(WishlistDto wishlistDto)
    {
        await _unitOfWork.Wishlists.DeleteAsync(wishlistDto.ToWishlist());
        return await _unitOfWork.CommitAsync();
    }

    public async Task<List<WishlistDto>> GetAllWishlistAsync(int userId)
    {
        var wishlist = await _unitOfWork.Wishlists.GetAllAsync(
            w => w.UserId == userId,
            q => q.Include(w => w.Product)
                  .ThenInclude(p => p.ProductPhotos.OrderBy(photo => photo.Id).Take(1))
                  .Include(w => w.Product.ProductFrames)
                  .ThenInclude(pf => pf.Frame)
        );

        return wishlist.Select(w => w.ToDTO()).ToList();
    }

}
