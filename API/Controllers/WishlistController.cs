using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly WishlistService _wishlistService;

        public WishlistController(WishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistDto wishlistDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            wishlistDto = wishlistDto with { UserId = userId };
            return Ok(await _wishlistService.AddToWishlistAsync(wishlistDto));
        }
        [HttpGet("GetWishlistforUser")]
        public async Task<IActionResult> GetWishlist()
        {

            // Check if the user is authenticated and extract userId from the token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            return Ok(await _wishlistService.GetAllWishlistAsync(userId));
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveFromWishlist([FromBody] WishlistDto wishlistDto)
        {
            return Ok(await _wishlistService.RemoveFromWishlistAsync(wishlistDto));
        }
    }
}
