using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ProductModels
{
    public class ProductPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; } // URL or path to the photo
        public string Description { get; set; } // Optional: description or alt text
        public int ProductId { get; set; } // Foreign key
        public virtual Product Product { get; set; }
    }
}
