namespace Domain.StaticData;

public class StaticData
{
    public const string CashPayment = "Cash On Delivery";
    public const string BankTransfer = "Direct Bank Transfer";
    public const string PayPal = "PayPal";
    public const string AdminRole = "Admin";
    public const string UserRole = "User";
    public const string OrderCreated = "Created";
    public const string OrderPacked = "Packed";
    public const string OrderShipped = "Shipped";
    public const string OrderDelivered = "Delivered";
    public static readonly List<string> OrderStatuses = new List<string> { "Created", "Packed", "Shipped", "Delivered" };

}