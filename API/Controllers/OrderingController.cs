using System.Security.Claims;
using Application.Services;
using Domain.StaticData;
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
    
    [HttpGet("Get-Every-Order-Paginated/{pageIndex:int}/{pageSize:int}")]
    [Authorize]
    public async Task<IActionResult> GetEveryOrder(int pageIndex, int pageSize)
    {
        var result = await _orderingService.GetAllOrders(pageIndex, pageSize);
        return Ok(result);
    }
    [HttpGet("Get-Every-Order")]
    public async Task<IActionResult> GetEveryOrder()
    {
        var result = await _orderingService.GetAllOrders();
        return Ok(result);
    }

    [HttpGet("Get-Past-Orders")]
    [Authorize]
    public async Task<IActionResult> GetPastOrders()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await _orderingService.ViewPastOrders(userId);
        return Ok(result);
    }

    [HttpPut("Update-OrderStatus{orderId:int}")]
    [Authorize(Roles = StaticData.AdminRole)]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, string orderStatus)
    {
        var result = await _orderingService.ChangeOrderStatus(orderId, orderStatus);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("Get-Order-Details{orderId:int}")]
    [Authorize]
    public async Task<IActionResult> GetOrderDetails(int orderId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        bool isAdmin = User.IsInRole(StaticData.AdminRole);
        var result = await _orderingService.GetOrderDetailsById(orderId, userId, isAdmin);
        if (result == null || (result.UserId != userId && !isAdmin))
        {
            return NoContent();
        }

        return Ok(result);
    }
}