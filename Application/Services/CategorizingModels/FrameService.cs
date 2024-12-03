using Application.DTOs;
using Application.DTOs.CategorizingModels;
using Application.DTOs.Mappers.CategorizingModels;
using Domain.Models.CategorizingModels;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;

namespace Application.Services;

public class FrameService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;

    public FrameService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
    {
        _environment = environment;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateFrameAsync(FrameDto frameDto)
    {
        var frame = frameDto.ToFrame();
        await _unitOfWork.Frames.AddAsync(frame);
        return await _unitOfWork.CommitAsync();
    }

    public async Task<IEnumerable<FrameDto>> GetAllFramesAsync()
    {
        var frames = await _unitOfWork.Frames.GetAllAsync();
        return frames.Select(f => f.ToDTO());
    }

    public async Task<FrameDto?> GetFrameByIdAsync(int id)
    {
        var frame = await _unitOfWork.Frames.GetAsync(f => f.Id == id);
        return frame?.ToDTO();
    }

    public async Task<int> UpdateFrameAsync(FrameDto frameDto)
    {
        var existingFrame = await _unitOfWork.Frames.GetAsync(f => f.Id == frameDto.Id);
        if (existingFrame == null) return 0;

        existingFrame.Name = frameDto.Name;

        await _unitOfWork.Frames.UpdateAsync(existingFrame);
        return await _unitOfWork.CommitAsync();
    }

    public async Task<int> DeleteFrameAsync(FrameDto frameDto)
    {
        var frame = frameDto.ToFrame();

        await _unitOfWork.Frames.DeleteAsync(frame);
        return await _unitOfWork.CommitAsync();
    }
}