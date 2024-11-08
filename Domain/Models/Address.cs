using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string AddressString { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public string StreetName { get; set; }
        public int BuildingNumber { get; set; }
        public virtual AppUser User { get; set; }

    }
}