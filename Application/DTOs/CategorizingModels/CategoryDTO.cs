using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CategorizingModels
{
    public record CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
