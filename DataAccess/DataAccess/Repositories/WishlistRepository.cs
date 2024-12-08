using Domain.Models;
using Domain.Models.ShopModels;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;

namespace Infrastructure.DataAccess.Repositories;

public class WishlistRepository : GenericRepository<WishList>, IWishlistRepository
{
    private readonly AppDbContext _context;

    public WishlistRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<IEnumerable<WishList>> GetAllAsync(
           Expression<Func<WishList, bool>>? predicate = null,
           params Func<IQueryable<WishList>, IIncludableQueryable<WishList, object>>[] includeProperties)
    {
        IQueryable<WishList> query = dbSet;

        if (predicate != null)
            query = query.Where(predicate);
        foreach (var include in includeProperties)
        {
            query = include(query);
        }
        return await query.ToListAsync();
    }
}