using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.ShopModels
{
    public class WishList
    {
        // Foreign key for AppUser
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }
        public DateTime AddedAt { get; set; }

        // Foreign key for Product
        public int ProductId { get; set; }

        [Required, ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
