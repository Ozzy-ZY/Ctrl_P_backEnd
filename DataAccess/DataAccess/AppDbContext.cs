using Domain.Models;
using Domain.Models.CategorizingModels;
using Domain.Models.ProductModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Frame> Frames { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductFrame> ProductFrames { get; set; }
        public DbSet<ProductMaterial> ProductMaterials { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AppUser> User { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Product).Assembly);
        }
    }
}