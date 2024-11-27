using Domain.Models;

namespace Infrastructure.DataAccess.Repositories;

public interface ICartRepository:IGenericRepository<Cart>
{
    public Task<Cart?> GetCartWithItemsAsync(int userId);
}