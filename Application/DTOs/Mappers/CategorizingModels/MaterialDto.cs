using Application.DTOs.CategorizingModels;
using Domain.Models.CategorizingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Mappers.CategorizingModels
{
    public static class MaterialMapper
    {
        public static MaterialDto ToDTO(this Material material)
    {
        return new MaterialDto(material.Id, material.Name);
    }

    public static Material ToMaterial(this MaterialDto materialDto)
    {
        return new Material
        {
            Id = materialDto.Id,
            Name = materialDto.Name
        };
    }
}
}
