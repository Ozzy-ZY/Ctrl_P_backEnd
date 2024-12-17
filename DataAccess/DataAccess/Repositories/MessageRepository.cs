using Domain.Models;

namespace Infrastructure.DataAccess.Repositories;

public class MessageRepository: GenericRepository<Message>
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}