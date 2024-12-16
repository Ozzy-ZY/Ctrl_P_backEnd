using Application.DTOs.CategorizingModels;
using Application.Services.CategorizingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.CategorizingControllers;

[Route("api/[controller]")]
[ApiController]
public class SizeController : ControllerBase
{
    private readonly SizeService _sizeService;

    public SizeController(SizeService sizeService)
    {
        _sizeService = sizeService;
    }

    [HttpPost("add")]
    [Authorize]
    public async Task<IActionResult> AddSize([FromBody] SizeDto sizeDto)
    {
        return Ok(await _sizeService.CreateSizeAsync(sizeDto));
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateSize([FromBody] SizeDto sizeDto)
    {
        return Ok(await _sizeService.UpdateSizeAsync(sizeDto));
    }

    [HttpGet("Get-All")]
    public async Task<IActionResult> GetAllSizes()
    {
        return Ok(await _sizeService.GetAllSizesAsync());
    }

    [HttpDelete("Delete")]
    [Authorize]
    public async Task<IActionResult> DeleteSize([FromBody] SizeDto sizeDto)
    {
        return Ok(await _sizeService.DeleteSizeAsync(sizeDto));
    }
}
