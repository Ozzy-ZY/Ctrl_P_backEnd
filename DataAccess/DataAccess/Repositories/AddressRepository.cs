using Domain.Models;

namespace Infrastructure.DataAccess.Repositories;

public class AddressRepository: GenericRepository<Address>
{
    private readonly AppDbContext _context;
    public AddressRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}