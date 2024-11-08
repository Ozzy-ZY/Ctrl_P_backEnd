using Domain.Models;

namespace Infrastructure.DataAccess.Repositories;

public class ServiceRepository: GenericRepository<Service>, IServiceRepository
{
    private readonly AppDbContext _context;

    public ServiceRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}