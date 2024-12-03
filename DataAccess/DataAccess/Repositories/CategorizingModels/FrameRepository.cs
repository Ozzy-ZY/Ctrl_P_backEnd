using Microsoft.EntityFrameworkCore;
using Domain.Models.CategorizingModels;

namespace Infrastructure.DataAccess.Repositories.CategorizingModels;

public class FrameRepository : GenericRepository<Frame>, IFrameRepository
{
    private readonly AppDbContext _context;

    public FrameRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
