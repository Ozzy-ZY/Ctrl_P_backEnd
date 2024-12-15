using Application.DTOs;
using Application.DTOs.Mappers;
using Domain.Models;
using Infrastructure.DataAccess;

namespace Application.Services;

public class AddressService
{
    private readonly IUnitOfWork _unitOfWork;

    public AddressService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Address>> GetAddressesAsync(int userId)
    {
        var result = await _unitOfWork.Addresses.GetAllAsync(a=> a.UserId == userId);
        return result;
    }

    public async Task<ServiceResult> AddAddressAsync(AddressDTO dto, int userId)
    {
        var result = new ServiceResult()
        {
            Success = false
        };
        var address = dto.AsAddress(userId);
        await _unitOfWork.Addresses.AddAsync(address);
        if (await _unitOfWork.CommitAsync() > 0)
        {
            await _unitOfWork.Addresses.UpdateAsync(address);
            result.Success = true;
            return result;
        }
        result.Errors.Add("Error saving address");
        return result;
    }

    public async Task<ServiceResult> UpdateAddressAsync(AddressUpdateDTO dto, int userId)
    {
        var result = new ServiceResult()
        {
            Success = false
        };
        var address = dto.AsAddress(userId);
        await _unitOfWork.Addresses.UpdateAsync(address);
        if (await _unitOfWork.CommitAsync() > 0)
        {
          result.Success = true;
          return result;
        }
        result.Errors.Add("Error Updating address");
        return result;
    }
}