using Domain.Models;
using Domain.Models.CategorizingModels;
using Infrastructure.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataAccess.Repositories.CategorizingModels;

public interface ICategoryRepository : IGenericRepository<Category>
{
}