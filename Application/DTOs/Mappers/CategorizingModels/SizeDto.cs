using Application.DTOs.CategorizingModels;
using Domain.Models.CategorizingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Mappers.CategorizingModels
{
    public static class SizeMapper
    {
        public static SizeDto ToDTO(this Size size)
        {
            return new SizeDto(size.Id, size.Name);
        }

        public static Size ToSize(this SizeDto sizeDto)
        {
            return new Size
            {
                Id = sizeDto.Id,
                Name = sizeDto.Name
            };
        }
    }
}
