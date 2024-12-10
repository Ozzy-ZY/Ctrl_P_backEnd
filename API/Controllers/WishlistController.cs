using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(await _wishlistService.AddToWishlistAsync(wishlistDto));
        }
        [HttpGet("GetWishlistfor/{Id}")]
        public async Task<IActionResult> GetWishlist(int Id)
        {
            return Ok(await _wishlistService.GetAllWishlistAsync(Id));
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveFromWishlist([FromBody] WishlistDto wishlistDto)
        {
            return Ok(await _wishlistService.RemoveFromWishlistAsync(wishlistDto));
        }
    }
}
