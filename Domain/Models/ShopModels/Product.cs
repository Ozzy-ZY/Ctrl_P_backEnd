using Domain.Models.ProductModels;

namespace Domain.Models
{
    public class Product
    {
        public int Id { get; set; }
        public ulong RowVersion { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool InStock { get; set; }
        public int InStockAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal Rating { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
        public virtual ICollection<ProductFrame> ProductFrames { get; set; }
        public virtual ICollection<ProductMaterial> ProductMaterials { get; set; }
        public virtual ICollection<ProductSize> ProductSizes { get; set; }
        public virtual ICollection<ProductPhoto> ProductPhotos { get; set; }
        public virtual ICollection<ProductReviews> ProductReviews { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }

    }
}