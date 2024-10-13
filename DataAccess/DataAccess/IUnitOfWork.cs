using Infrastructure.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess
{
    public interface IUnitOfWork
    {
        public IProductRepository Products { get; set; }

        public Task<int> CommitAsync();
    }
}