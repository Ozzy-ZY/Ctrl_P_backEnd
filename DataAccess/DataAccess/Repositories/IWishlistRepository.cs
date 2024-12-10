using Domain.Models;
using Domain.Models.ShopModels;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.DataAccess.Repositories;

public interface IWishlistRepository : IGenericRepository<WishList>
{
    public Task<IEnumerable<WishList>> GetAllAsync(Expression<Func<WishList, bool>>? predicate = null,
  params Func<IQueryable<WishList>, IIncludableQueryable<WishList, object>>[] includeProperties);
}