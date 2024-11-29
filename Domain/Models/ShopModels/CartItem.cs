using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Models;

public class CartItem
{
    [Key]
    public int Id { get; set; }
    public ulong RowVersion { get; set; }
    
    [Required, ForeignKey("Cart")]
    public int CartId { get; set; }
    
    [Required, ForeignKey("Product")]
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; }
    [JsonIgnore]
    public virtual Cart Cart { get; set; }
    [JsonIgnore]
    public virtual Product Product { get; set; }
}