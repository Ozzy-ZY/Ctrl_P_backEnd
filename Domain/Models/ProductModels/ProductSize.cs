﻿using Domain.Models.CategorizingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ProductModels
{
    public class ProductSize
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int SizeId { get; set; }
        public virtual Size Size { get; set; }
    }
}
