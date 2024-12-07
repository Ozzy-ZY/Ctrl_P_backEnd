using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.ProductModels
{
    public class ProductReviews
    {
        [Key]
        public int Id { get; set; }
        [Required, ForeignKey("AppUser")]
        public int ReviewerId { get; set; }
        [Required, ForeignKey("Product")]
        public int ProductId { get; set; }
        public required string Review { get; set; }
        public required decimal Rating { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public DateTime ReviewDate { get; set; }
        public virtual AppUser Reviewer { get; set; }
        public virtual Product Product { get; set; }
    }
}