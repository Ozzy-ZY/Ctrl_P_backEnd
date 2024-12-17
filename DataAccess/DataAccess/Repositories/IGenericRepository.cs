using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Pagination;

namespace Infrastructure.DataAccess.Repositories
{
    public interface IGenericRepository<T>
        where T : class
    {
        public Task<T?> GetAsync(Expression<Func<T, bool>>? predicate = null, params Expression<Func<T, object>>[] includeProperties);
        public Task<PaginatedList<T>> GetPaginatedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            params Expression<Func<T, object>>[] includeProperties);

        public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
            params Expression<Func<T, object>>[] includeProperties);

        public Task AddAsync(T Entity);
        public Task AddBulkAsync(IEnumerable<T> Entities);
        public Task UpdateAsync(T Entity);

        public Task DeleteAsync(T Entity);
        public Task DeleteAllAsync(Expression<Func<T, bool>>? predicate = null);
    }
}