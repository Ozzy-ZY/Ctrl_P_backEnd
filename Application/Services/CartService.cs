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
    public class AddToCartResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
       public async Task<AddToCartResult> AddToCartAsync(int userId, AddToCartDTO addToCartDTO)
    {
        var result = new AddToCartResult();

        try
        {
            // Validate input
            if (addToCartDTO == null)
            {
                result.Errors.Add("Cart item cannot be null");
                return result;
            }

            if (addToCartDTO.Quantity <= 0)
            {
                result.Errors.Add("Quantity must be greater than zero");
                return result;
            }

            // Retrieve product
            var product = await _unitOfWork.Products.GetAsync(p => p.Id == addToCartDTO.ProductId);
            if (product == null)
            {
                result.Errors.Add($"Product with ID {addToCartDTO.ProductId} not found");
                return result;
            }

            // Check product availability
            if (product.InStockAmount < addToCartDTO.Quantity)
            {
                result.Errors.Add($"Insufficient stock. Only {product.InStockAmount} items available");
                return result;
            }

            // Retrieve or create cart
            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _unitOfWork.Carts.AddAsync(cart);
            }

            // Check for existing cart item
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == addToCartDTO.ProductId);
            
            // Validate total quantity
            var totalRequestedQuantity = addToCartDTO.Quantity + 
                (cartItem?.Quantity ?? 0);
            
            if (totalRequestedQuantity > product.InStockAmount)
            {
                result.Errors.Add($"Cannot add {addToCartDTO.Quantity} items. Total would exceed available stock of {product.InStockAmount}");
                return result;
            }

            // Add or update cart item
            if (cartItem == null)
            {
                cartItem = addToCartDTO.ToCartItem(cart.Id);
                cart.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += addToCartDTO.Quantity;
            }

            // Save changes
            await _unitOfWork.Carts.UpdateAsync(cart);
            await _unitOfWork.CommitAsync();

            result.Success = true;
            return result;
        }
        catch (Exception ex)
        {
            result.Errors.Add("An unexpected error occurred while adding item to cart");
            
            return result;
        }
    }
}