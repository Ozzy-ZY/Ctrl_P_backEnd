using System.Security.Claims;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrderingController: ControllerBase
{
    private readonly OrderingService _orderingService;

    public OrderingController(OrderingService orderingService)
    {
        _orderingService = orderingService;
    }

    [HttpPost("add-order")]
    [Authorize]
    public async Task<IActionResult> AddOrder()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await _orderingService.CreateOrderFromCart(userId);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("get-orders")]
    [Authorize]
    public async Task<IActionResult> GetOrders()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await _orderingService.ViewPastOrders(userId);
        return Ok(result);
    }
}