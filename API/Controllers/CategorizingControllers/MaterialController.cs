using Application.DTOs.CategorizingModels;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.CategorizingControllers;

[Route("api/[controller]")]
[ApiController]
public class MaterialController : ControllerBase
{
    private readonly MaterialService _materialService;

    public MaterialController(MaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpPost("add")]
    [Authorize]
    public async Task<ActionResult<string>> AddMaterial([FromBody] MaterialDto materialDto)
    {
        return (await _materialService.CreateMaterialAsync(materialDto)).ToString();
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<ActionResult<string>> UpdateMaterial([FromBody] MaterialDto materialDto)
    {
        return (await _materialService.UpdateMaterialAsync(materialDto)).ToString();
    }

    [HttpGet("Get-All")]
    public async Task<IActionResult> GetAllMaterials()
    {
        return Ok(await _materialService.GetAllMaterialsAsync());
    }

    [HttpDelete("Delete")]
    [Authorize]
    public async Task<IActionResult> DeleteMaterial([FromBody] MaterialDto materialDto)
    {
        return Ok(await _materialService.DeleteMaterialAsync(materialDto));
    }
}
