using Domain.Models;

namespace Infrastructure.DataAccess.Repositories;

public class OrderItemRepository: GenericRepository<OrderItem>
{
    private readonly AppDbContext _context;
    public OrderItemRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}