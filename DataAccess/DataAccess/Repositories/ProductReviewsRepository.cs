using Domain.Models.ProductModels;

namespace Infrastructure.DataAccess.Repositories;

public class ProductReviewsRepository : GenericRepository<ProductReviews>, IProductReviewsRepository
{
    private readonly AppDbContext _context;
    public ProductReviewsRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

}

