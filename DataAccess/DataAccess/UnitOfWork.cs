using Domain.Models;
using Infrastructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public IProductRepository Products { get; set; }

        public UnitOfWork(AppDbContext context, UserManager<AppUser> usermanager)
        {
            _context = context;
            _userManager = usermanager;
            Products = new ProductRepository(context);
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}