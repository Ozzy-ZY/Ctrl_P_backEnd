using Application.DTOs;
using Application.DTOs.Mappers;
using Domain.Models;
using Infrastructure.DataAccess;
using Infrastructure.DataAccess.Repositories;

namespace Application.Services;

public class CartService
{
    private readonly IUnitOfWork _unitOfWork;

    public CartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CartDTO> GetCartWithItemsAsync(int userId)
    {
        var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
        if (cart == null)
        {
            return null!;
        }
        return cart.ToCartDTO();
    }

    public async Task<bool> AddToCartAsync(int userId, AddToCartDTO addToCartDTO)
    {
        try
        {
            var product = await _unitOfWork.Products.GetAsync(p => p.Id == addToCartDTO.ProductId);
            if (product == null || product.InStockAmount < addToCartDTO.Quantity)
                return false;
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
            {
                cart = new Cart() { UserId = userId };
                await _unitOfWork.Carts.AddAsync(cart);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == addToCartDTO.ProductId);
            if (cartItem == null)
            {
                cartItem = addToCartDTO.ToCartItem(cart.Id);
                cart.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += addToCartDTO.Quantity;
            }

            await _unitOfWork.Carts.UpdateAsync(cart);
            return true;
        }
        catch (Exception ex)
        {
            // add a logger later baby
            return false;
        }
    }
}