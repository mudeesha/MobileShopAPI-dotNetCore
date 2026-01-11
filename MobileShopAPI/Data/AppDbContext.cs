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
        
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        
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
            
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasOne(ci => ci.Cart)
                    .WithMany(c => c.Items)
                    .HasForeignKey(ci => ci.CartId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ci => ci.Product)
                    .WithMany()
                    .HasForeignKey(ci => ci.ProductId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent deleting product if it's in a cart

                entity.HasIndex(ci => new { ci.CartId, ci.ProductId })
                    .IsUnique(); // Ensure one product per cart
            });

            // Cart configuration
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => c.UserId)
                    .IsUnique(); // One cart per user
            });
        }
    }
}