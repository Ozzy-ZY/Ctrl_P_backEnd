using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record ProductDTO(string Name, string Description, int UnitsInStock, string Category, string ImageUrl);
}