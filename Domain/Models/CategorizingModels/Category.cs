﻿using Domain.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.CategorizingModels
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., Electronics, Furniture, etc.
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}