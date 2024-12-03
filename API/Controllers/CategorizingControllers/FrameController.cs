using Application.DTOs.CategorizingModels;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.CategorizingControllers;

[Route("api/[controller]")]
[ApiController]
public class FrameController : ControllerBase
{
    private readonly FrameService _frameService;
    public FrameController(FrameService frameService)
    {
        _frameService = frameService;
    }
    [HttpPost("add")]
    [Authorize]
    public async Task<ActionResult<string>> AddFrame([FromBody] FrameDto frameDto)
    {
        return (await _frameService.CreateFrameAsync(frameDto)).ToString();
    }
    [HttpPut("update")]
    [Authorize]
    public async Task<ActionResult<string>> UpdateFrame([FromBody] FrameDto frameDto)
    {
        return (await _frameService.UpdateFrameAsync(frameDto)).ToString();
    }
    [HttpGet("Get-All")]
    public async Task<IActionResult> GetAllFrames()
    {
        return Ok(await _frameService.GetAllFramesAsync());
    }
    [HttpDelete("Delete")]
    [Authorize]
    public async Task<IActionResult> DeleteFrame([FromBody] FrameDto frameDto)
    {
        return Ok(await _frameService.DeleteFrameAsync(frameDto));
    }
}
