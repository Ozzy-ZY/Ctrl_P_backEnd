using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess.Repositories;

public class CartRepository:GenericRepository<Cart>,ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Cart?> GetCartWithItemsAsync(int userId)
    {
        return (await _context.Carts.Include(c=> c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId))!;
    }
}