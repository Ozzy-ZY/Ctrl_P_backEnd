using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ServiceController
{
    private readonly ServicesService _serviceService;

    public ServiceController(ServicesService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpPost("api/services/add")]
    public async Task<ActionResult<string>> AddService([FromForm] ServiceDTO service)
    {
        return (await _serviceService.AddService(service)).ToString();
    }
    [HttpPut("api/services/update")]
    public async Task<ActionResult<string>> UpdateService([FromForm] ServiceDTO service)
    {
        return (await _serviceService.UpdateServiceAsync(service)).ToString();
    }
}