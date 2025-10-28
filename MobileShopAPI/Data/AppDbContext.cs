using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Models;

namespace MobileShopAPI.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
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
        public DbSet<ProductImage> ProductImages { get; set; }
        
        // ✅ ADD THIS MISSING DbSet
        public DbSet<ProductImageAssignment> ProductImageAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price)
                      .HasPrecision(18, 2);
            });

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
            
            modelBuilder.Entity<ProductImageAssignment>(entity =>
            {
                // Composite primary key
                entity.HasKey(pia => new { pia.ProductId, pia.ProductImageId });

                // Relationship with Product
                entity.HasOne(pia => pia.Product)
                    .WithMany(p => p.ProductImageAssignments)
                    .HasForeignKey(pia => pia.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with ProductImage
                entity.HasOne(pia => pia.ProductImage)
                    .WithMany(pi => pi.ProductImageAssignments)
                    .HasForeignKey(pia => pia.ProductImageId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}