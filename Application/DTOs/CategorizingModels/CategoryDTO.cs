using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace Application.DTOs.CategorizingModels
{
    public record CategoryDto(int Id, string Name, IFormFile Image, string? ImageUrl);
    public record CategoryDtoDelete(int Id);
}