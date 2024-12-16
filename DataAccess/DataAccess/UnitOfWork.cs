using Domain.Models;
using Infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Infrastructure.DataAccess.Repositories.CategorizingModels;
using Domain.Models.ShopModels;

namespace Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork, IAsyncDisposable
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public IProductRepository Products { get; set; }
        public ICartRepository Carts { get; set; }
        public IServiceRepository Services { get; set; }
        public ICategoryRepository Categories { get; set; }
        public IFrameRepository Frames { get; set; }
        public IMaterialRepository Materials { get; set; }
        public ISizeRepository Sizes { get; set; }
        public IProductReviewsRepository ProductReviews { get; set; }
        public IWishlistRepository Wishlists { get; set; }
        public OrderRepository Orders { get; set; }
        public OrderItemRepository OrderItems { get; set; }
        public AddressRepository Addresses { get; set; }
        public UserRepository Users { get; set; }
        public ICartItemRepository CartItems { get; set; }

        public UnitOfWork(AppDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            Products = new ProductRepository(context);
            Carts = new CartRepository(context);
            Services = new ServiceRepository(context);
            CartItems = new CartItemRepository(context);
            Categories = new CategoryRepository(context);
            Frames = new FrameRepository(context);
            Materials = new MaterialRepository(context);
            Sizes = new SizeRepository(context);
            ProductReviews = new ProductReviewsRepository(context);
            Wishlists = new WishlistRepository(context);
            Orders = new OrderRepository(context);
            OrderItems = new OrderItemRepository(context);
            Addresses = new AddressRepository(context);
            Users = new UserRepository(context);
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbTransaction> BeginTransactionAsync()
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            return transaction.GetDbTransaction();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
            await CastAndDispose(_userManager);
            await CastAndDispose(_roleManager);

            return;

            static async ValueTask CastAndDispose(IDisposable resource)
            {
                if (resource is IAsyncDisposable resourceAsyncDisposable)
                    await resourceAsyncDisposable.DisposeAsync();
                else
                    resource.Dispose();
            }
        }
    }
}