using Infrastructure.DataAccess.Repositories;
using Infrastructure.DataAccess.Repositories.CategorizingModels;
using System.Data;

namespace Infrastructure.DataAccess
{
    public interface IUnitOfWork
    {
        public IProductRepository Products { get; set; }
        public ICartRepository Carts { get; set; }
        public IServiceRepository Services { get; set; }
        public ICartItemRepository CartItems { get; set; }
        public ICategoryRepository Categories { get; set; }
        public IFrameRepository Frames { get; set; }
        public IMaterialRepository Materials { get; set; }
        public ISizeRepository Sizes { get; set; }
        public IProductReviewsRepository ProductReviews { get; set; }
        public OrderRepository Orders { get; set; }
        public OrderItemRepository OrderItems { get; set; }

        public Task<int> CommitAsync();
        public Task<IDbTransaction> BeginTransactionAsync();
    }
}