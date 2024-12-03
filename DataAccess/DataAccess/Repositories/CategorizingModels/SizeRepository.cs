using Domain.Models.CategorizingModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess.Repositories.CategorizingModels
{
    public class SizeRepository : GenericRepository<Size>, ISizeRepository
    {
        private readonly AppDbContext _context;

        public SizeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
