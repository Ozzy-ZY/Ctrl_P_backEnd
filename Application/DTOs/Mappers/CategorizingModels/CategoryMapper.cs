using Application.DTOs.CategorizingModels;
using Domain.Models.CategorizingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Mappers.CategorizingModels
{
    public static class CategoryMapper
    {
        public static CategoryDto ToDTO(this Category category)
        {
            return new CategoryDto(category.Id, category.Name, null!, category.ImageUrl);
        }
        public static Category ToCategory(this CategoryDto categoryDto, string url)
        {
            return new Category
            {
                Id = categoryDto.Id,
                Name = categoryDto.Name,
                ImageUrl = url
            };
        }
        public static CategoryDtoDelete ToDTODelete(this Category category)
        {
            return new CategoryDtoDelete(category.Id);
        }
    }
}
