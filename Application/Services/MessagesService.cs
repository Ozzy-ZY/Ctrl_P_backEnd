using Application.DTOs;
using Application.DTOs.Mappers;
using Domain.Models;
using Infrastructure.DataAccess;

namespace Application.Services;

public class MessagesService
{
    private readonly IUnitOfWork _unitOfWork;

    public MessagesService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Message>> GetAllMessagesAsync(int pageNumber = 1, int pageSize = 10)
    {
        var messages = (await _unitOfWork.Messages.GetPaginatedAsync(pageNumber, pageSize)).Items;
        return messages;
    }

    public async Task<ServiceResult> SendMessageAsync(SendMessageDTO dto, int userId)
    {
        var result = new ServiceResult()
        {
            Success = false
        };
        var message = dto.ToMessage(userId);
        await _unitOfWork.Messages.AddAsync(message);
        if (await _unitOfWork.CommitAsync() > 0)
        {
            result.Success = true;
            return result;
        }
        result.Errors.Add("Error Sending the message Please Try again");
        return result;
    }
}