namespace MobileShopAPI.Data
{
    using Microsoft.EntityFrameworkCore;
    using MobileShopAPI.Models;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AttributeType> AttributeTypes { get; set; }
        public DbSet<AttributeValue> AttributeValues { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for ProductAttribute junction table
            modelBuilder.Entity<ProductAttribute>()
            .HasKey(pa => new { pa.ProductId, pa.AttributeValueId });

            modelBuilder.Entity<ProductAttribute>()
                .HasOne(pa => pa.Product)
                .WithMany(p => p.ProductAttributes)
                .HasForeignKey(pa => pa.ProductId);

            modelBuilder.Entity<ProductAttribute>()
                .HasOne(pa => pa.AttributeValue)
                .WithMany(av => av.ProductAttributes)
                .HasForeignKey(pa => pa.AttributeValueId);
        }
    }

}
