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
        public DbSet<ProductInventory> ProductInventories { get; set; }
        public DbSet<InventoryAttributeValue> InventoryAttributeValues { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            modelBuilder.Entity<InventoryAttributeValue>()
                .HasKey(iav => new { iav.ProductInventoryId, iav.AttributeValueId });

            modelBuilder.Entity<InventoryAttributeValue>()
                .HasOne(iav => iav.ProductInventory)
                .WithMany(pi => pi.InventoryAttributeValues)
                .HasForeignKey(iav => iav.ProductInventoryId);

            modelBuilder.Entity<InventoryAttributeValue>()
                .HasOne(iav => iav.AttributeValue)
                .WithMany()
                .HasForeignKey(iav => iav.AttributeValueId);

            modelBuilder.Entity<ProductImageAttributeValue>()
                .HasKey(p => new { p.ProductImageId, p.AttributeValueId });

            modelBuilder.Entity<ProductImageAttributeValue>()
                .HasOne(p => p.ProductImage)
                .WithMany(i => i.ProductImageAttributeValues)
                .HasForeignKey(p => p.ProductImageId);




        }
    }
}
