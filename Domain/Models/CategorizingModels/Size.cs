using Domain.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.CategorizingModels
{
    public class Size
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "Small", "Medium", "Large"
        public virtual ICollection<ProductSize> ProductSizes { get; set; }
    }
}
