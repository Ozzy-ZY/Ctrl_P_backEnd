using System.Security.Claims;
using Application.DTOs;
using Application.Services;
using Domain.Models;
using Domain.StaticData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly AddressService _addressService;

    public AddressController(AddressService addressService)
    {
        _addressService = addressService;
    }
    [HttpPost("Add-Address")]
    [Authorize(Roles = StaticData.UserRole)]
    public async Task<IActionResult> AddAddress(AddressDTO dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await _addressService.AddAddressAsync(dto, userId);
        if(result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPut("Update-Address")]
    [Authorize(Roles = StaticData.UserRole)]
    public async Task<IActionResult> UpdateAddress(AddressUpdateDTO dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await _addressService.UpdateAddressAsync(dto, userId);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpGet("Get-Addresses{userId:int}")]
    public async Task<IActionResult> GetAddresses(int userId)
    {
        var result = await _addressService.GetAddressesAsync(userId);
        return Ok(result);
    }
}