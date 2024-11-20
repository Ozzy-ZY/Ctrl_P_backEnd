using Application.DTOs;
using Application.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServiceController : ControllerBase
{
    private readonly ServicesService _serviceService;

    public ServiceController(ServicesService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpPost("add")]
    [Authorize]
    public async Task<ActionResult<string>> AddService([FromForm] ServiceDTO service)
    {
        return (await _serviceService.AddService(service)).ToString();
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<ActionResult<string>> UpdateService([FromForm] ServiceDTO service)
    {
        return (await _serviceService.UpdateServiceAsync(service)).ToString();
    }

    [HttpGet("Get-All")]
    public async Task<IActionResult> GetAllServices()
    {
        return Ok(await _serviceService.GetAllServicesAsync());
    }
}