using Domain.Models;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        public Task<IEnumerable<Product>> GetAllAsync(Expression<Func<Product, bool>>? predicate = null,
    params Func<IQueryable<Product>, IIncludableQueryable<Product, object>>[] includeProperties);
        public Task<Product?> GetAsync(Expression<Func<Product, bool>>? predicate = null,
    params Func<IQueryable<Product>, IIncludableQueryable<Product, object>>[] includeProperties);

            IQueryable<Product> Query();


    }
}