using Application.DTOs;
using Application.Services;
using Domain.Models;
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
    [HttpPost("Add-Address{userId:int}")]
    public async Task<IActionResult> AddAddress(AddressDTO dto, int userId)
    {
        var result = await _addressService.AddAddressAsync(dto, userId);
        if(result.Success)
            return Ok(result);
        return BadRequest(result);
    }
}