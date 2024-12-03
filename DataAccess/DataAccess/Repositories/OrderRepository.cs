using Domain.Models;

namespace Infrastructure.DataAccess.Repositories;

public class OrderRepository: GenericRepository<Order>
{
    private readonly AppDbContext _context;
    public OrderRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}