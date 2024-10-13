using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string AddressString { get; set; }
        public string StreetName { get; set; }
        public int BuildingNumber { get; set; }
    }
}