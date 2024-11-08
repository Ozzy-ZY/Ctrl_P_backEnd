using Domain.Models;

namespace Infrastructure.DataAccess.Repositories;

public class CartRepository:GenericRepository<Cart>,ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}