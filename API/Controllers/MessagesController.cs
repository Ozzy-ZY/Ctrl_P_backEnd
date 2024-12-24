using System.Security.Claims;
using Application.DTOs.Mappers;
using Application.Services;
using Domain.StaticData;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MessagesController: ControllerBase
{
    private readonly MessagesService _messagesService;
    private readonly IValidator<SendMessageDTO> _messageValidator;

    public MessagesController(MessagesService messagesService, IValidator<SendMessageDTO> messageValidator)
    {
        _messagesService = messagesService;
        _messageValidator = messageValidator;
    }

    [HttpGet("GetMessagesPage/")]
    [Authorize(Roles = StaticData.AdminRole)]
    public async Task<IActionResult> GetMessages()
    {
        var result = await _messagesService.GetAllMessagesAsync();
        if (result.Any())
        {
            return Ok(result);
        }
        return NoContent();
    }

    [HttpPost("SendMessage/")]
    [Authorize(Roles = StaticData.UserRole)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO message)
    {
        var modelState = await _messageValidator.ValidateAsync(message);
        if (!modelState.IsValid)
        {
            return BadRequest(modelState.Errors);
        }
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await _messagesService.SendMessageAsync(message,userId);
        return Ok(result);
    }
}