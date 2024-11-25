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
        public static CategoryDTO ToDTO(this Category category)
        {
            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public static Category ToCategory(this CategoryDTO categoryDto)
        {
            return new Category
            {
                Id = categoryDto.Id,
                Name = categoryDto.Name
            };
        }

        public static List<CategoryDTO> ToDTOList(this IEnumerable<Category> categories)
        {
            return categories.Select(c => c.ToDTO()).ToList();
        }
    }
}
