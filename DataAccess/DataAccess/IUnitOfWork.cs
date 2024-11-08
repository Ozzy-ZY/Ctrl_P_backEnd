using Infrastructure.DataAccess.Repositories;
using System.Data;

namespace Infrastructure.DataAccess
{
    public interface IUnitOfWork
    {
        public IProductRepository Products { get; set; }
        public ICartRepository Carts { get; set; }
        public IServiceRepository Services { get; set; }
        public ICartItemRepository CartItems { get; set; }

        public Task<int> CommitAsync();
        public Task<IDbTransaction> BeginTransactionAsync();
    }
}