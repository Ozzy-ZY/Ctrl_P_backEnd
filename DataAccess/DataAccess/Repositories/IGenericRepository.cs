﻿using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess.Repositories
{
    public interface IGenericRepository<T>
        where T : class
    {
        public Task<T> GetAsync(Expression<Func<T, bool>>? predicate = null, params string[] Includes);

        public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, params string[] Includes);

        public Task AddAsync(T Entity);

        public Task UpdateAsync(T Entity);

        public Task DeleteAsync(T Entity);
    }
}