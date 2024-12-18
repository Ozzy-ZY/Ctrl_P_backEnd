using Domain.Models;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(
     Expression<Func<Product ,bool>>? predicate = null,
     params Func<IQueryable<Product>, IIncludableQueryable<Product, object>>[] includeProperties)
        {
            IQueryable<Product> query = dbSet;

            // Apply the predicate if provided
            if (predicate != null)
                query = query.Where(predicate);

            // Apply the include properties for eager loading
            foreach (var include in includeProperties)
                query = include(query);

            // Return the list of entities
            return await query.ToListAsync();
        }

        public async Task<Product?> GetAsync(
            Expression<Func<Product, bool>>? predicate = null,
            params Func<IQueryable<Product>, IIncludableQueryable<Product, object>>[] includeProperties)
        {
            IQueryable<Product> query = dbSet;

            // Apply the predicate if provided
            if (predicate != null)
                query = query.Where(predicate);

            // Apply the include properties for eager loading
            foreach (var include in includeProperties)
                query = include(query);

            // Return the first or default entity matching the query
            return await query.FirstOrDefaultAsync();
        }
        public IQueryable<Product> Query()
        {
            return _context.Products.AsQueryable();
        }

    }
}