using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public class Cart
{
    [Key]
    public int Id { get; set; }
    
    [Required, ForeignKey("AppUser")]
    public int UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public virtual AppUser AppUser { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; }

}