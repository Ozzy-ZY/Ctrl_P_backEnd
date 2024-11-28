using Application.DTOs;
using Application.DTOs.Mappers;
using Domain.Models;
using Infrastructure.DataAccess;
using Infrastructure.DataAccess.Repositories;
using Microsoft.VisualBasic;

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
        var cart = await _unitOfWork.Carts.GetAsync(c => c.UserId == userId, c => c.CartItems);
        return cart?.ToCartDTO()!;
    }
    public class AddToCartResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public async Task<AddToCartResult> AddToCartAsync(int userId, AddToCartDTO addToCartDTO)
    {
        AddToCartResult result = new AddToCartResult();
        var product = await _unitOfWork.Products.GetAsync(p => p.Id == addToCartDTO.ProductId);
        if (addToCartDTO.Quantity > product!.InStockAmount)
        {
            result.Success = false;
            result.Errors.Add("Quantity must be greater than the product's Stock");
            return result;
        }
        // get the user's cart
        var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
        if (cart == null)
            return result;
        // check if the cart item already exists
        var existingCartItem = await _unitOfWork.CartItems.GetAsync(i => (i.CartId == cart!.Id && i.ProductId == addToCartDTO.ProductId));
        if (existingCartItem == null)
        {
            existingCartItem = addToCartDTO.ToCartItem(cart!.Id);
            await _unitOfWork.CartItems.AddAsync(existingCartItem);
            cart.TotalPrice += addToCartDTO.Quantity * product!.Price;
        }
        else
        {
            existingCartItem.Quantity += addToCartDTO.Quantity;
            product!.InStockAmount -= addToCartDTO.Quantity;
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.CartItems.UpdateAsync(existingCartItem);
        }
        cart.TotalPrice += addToCartDTO.Quantity * product!.Price;
        await _unitOfWork.Carts.UpdateAsync(cart);
        if (await _unitOfWork.CommitAsync() > 0)
        {
            result.Success = true;
            return result;
        }
        
        result.Errors.Add("Unable to add product");
        result.Success = false;
        return result;
    }

    public async Task<bool> RemoveFromCartAsync(int userId, AddToCartDTO addToCartDTO)
    {
        var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
        if(cart == null)
            return false;
        var cartItem = await _unitOfWork.CartItems.GetAsync(ci => ci.CartId == cart.Id);
        if (cartItem == null)
            return false;
        await _unitOfWork.CartItems.DeleteAsync(cartItem!);
        var product = await _unitOfWork.Products.GetAsync(p => p.Id == addToCartDTO.ProductId);
        product!.InStockAmount += addToCartDTO.Quantity;
        await _unitOfWork.Products.UpdateAsync(product);
        if (await _unitOfWork.CommitAsync() > 0)
            return true;
        return false;
    }
}