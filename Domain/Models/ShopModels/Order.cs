namespace Domain.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public int UserId { get; set; }
    public string OrderStatus { get; set; }
    public string PaymentMethod { get; set; }
    public int AddressId { get; set; }
    public Address Address { get; set; }
    public AppUser AppUser { get; set; }
    public ulong RowVersion { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
    
    
}