using Domain.Models;

namespace Application.DTOs.Mappers;

public static class MessageMapper
{
    public static Message ToMessage(this SendMessageDTO dto, int userId)
    {
        return new Message()
        {
            UserId = userId,
            ContactEmail = dto.ContactEmail,
            Content = dto.Content,
            Subject = dto.Subject
        };
    }
}