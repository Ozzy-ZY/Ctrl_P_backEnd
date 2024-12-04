using Infrastructure.DataAccess;

namespace Application.Services;

public class OrderingService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
}