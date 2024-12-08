using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AddressText { get; set; }
        public string BillingAddress { get; set; }
        public string FullName { get; set; }
        public string? CompanyName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string StreetAddress { get; set; }
        public string Note { get; set; }
        [JsonIgnore]
        public virtual AppUser User { get; set; }

    }
}