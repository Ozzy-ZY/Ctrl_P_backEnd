using Domain.Models;

namespace Infrastructure.DataAccess.Repositories;

public class CartItemRepository:GenericRepository<CartItem>,ICartItemRepository
{
    private readonly AppDbContext _context;
    public CartItemRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}