using System.Security.Claims;
using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CartController: ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("GetCartWithItems")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetCartWithItems()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var cart = await _cartService.GetCartWithItemsAsync(userId);
        return Ok(cart);
    }
    [HttpPost("GetCartItems")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> AddItemToCart(AddToCartDTO request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        return Ok(await _cartService.AddToCartAsync(userId, request));
    }
}