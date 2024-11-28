using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        private readonly AppDbContext _context;
        protected DbSet<T> dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T Entity)
        {
            await dbSet.AddAsync(Entity);
        }

        public Task DeleteAsync(T Entity)
        {
            dbSet.Remove(Entity);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet;

            if (predicate != null)
                query = query.Where(predicate);
            foreach (var include in includeProperties)
            {
                await query.Include(include).LoadAsync();
            }
            return await query.ToListAsync();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>>? predicate = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet;

            if (predicate != null)
                query = query.Where(predicate);
            foreach (var include in includeProperties)
            {
                await query.Include(include).LoadAsync();
            }
            return await query.FirstOrDefaultAsync();
        }

        public Task UpdateAsync(T Entity)
        {
            dbSet.Attach(Entity);
            _context.Entry(Entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }
    }
}