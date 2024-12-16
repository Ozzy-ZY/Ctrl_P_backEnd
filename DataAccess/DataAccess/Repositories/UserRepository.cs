using Domain.Models;

namespace Infrastructure.DataAccess.Repositories;

public class UserRepository: GenericRepository<AppUser>
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}