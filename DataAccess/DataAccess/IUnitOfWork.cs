using Infrastructure.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.DataAccess
{
    public interface IUnitOfWork
    {
        public IProductRepository Products { get; set; }

        public Task<int> CommitAsync();
        public Task<IDbTransaction> BeginTransactionAsync();
    }
}