using Domain.Models.CategorizingModels;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.DataAccess.Repositories.CategorizingModels;

public class MaterialRepository : GenericRepository<Material>, IMaterialRepository
{
    private readonly AppDbContext _context;

    public MaterialRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}

