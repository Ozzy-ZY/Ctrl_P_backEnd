using Domain.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.CategorizingModels
{
    public class Frame
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "Metal", "Plastic", etc.
        public virtual ICollection<ProductFrame> ProductFrames { get; set; }
    }
}
