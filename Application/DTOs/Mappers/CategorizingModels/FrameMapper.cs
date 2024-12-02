using Application.DTOs.CategorizingModels;
using Domain.Models.CategorizingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Mappers.CategorizingModels
{
    public static class FrameMapper
    {
        public static FrameDto ToDTO(this Frame frame)
        {
            return new FrameDto(frame.Id, frame.Name);
        }

        public static Frame ToFrame(this FrameDto frameDto)
        {
            return new Frame
            {
                Id = frameDto.Id,
                Name = frameDto.Name
            };
        }
    }
}
