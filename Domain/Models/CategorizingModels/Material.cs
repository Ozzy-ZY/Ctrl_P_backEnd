using Domain.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.CategorizingModels
{
    public class Material
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "Wood", "Glass", etc.
        public virtual ICollection<ProductMaterial> ProductMaterials { get; set; }
    }
}
