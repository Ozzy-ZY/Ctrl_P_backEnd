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
    public async Task<IActionResult> AddService([FromForm] ServiceDTO service)
    {
        return Ok(await _serviceService.AddService(service));
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateService([FromForm] ServiceDTO service)
    {
        return Ok(await _serviceService.UpdateServiceAsync(service));
    }

    [HttpGet("Get-All")]
    public async Task<IActionResult> GetAllServices(int pageNumber = 1, int pageSize = 5)
    {
        return Ok(await _serviceService.GetAllServicesAsync(pageNumber, pageSize));
    }
    [HttpDelete("Delete")]
    [Authorize]
    public async Task<IActionResult> DeleteService(int id)
    {
        return Ok(await _serviceService.DeleteServiceAsync(id));
    }
}