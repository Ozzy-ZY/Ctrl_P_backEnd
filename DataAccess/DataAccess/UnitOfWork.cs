using Domain.Models;
using Infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork, IAsyncDisposable
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public IProductRepository Products { get; set; }

        public UnitOfWork(AppDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            Products = new ProductRepository(context);
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