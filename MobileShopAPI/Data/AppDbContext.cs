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
        public DbSet<ProductImageAssignment> ProductImageAssignments { get; set; }
        
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderAddress> OrderAddresses { get; set; } 
        
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<CashOnDelivery> CashOnDeliveries { get; set; }

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
                entity.HasKey(pia => new { pia.ProductId, pia.ProductImageId });
                
                entity.HasOne(pia => pia.Product)
                    .WithMany(p => p.ProductImageAssignments)
                    .HasForeignKey(pia => pia.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                
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
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(ci => new { ci.CartId, ci.ProductId })
                    .IsUnique(); // Ensure one product per cart
            });
            
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => c.UserId)
                    .IsUnique(); // One cart per user
            });
            
            // Configure enums to be stored as integers
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<int>();
                
            modelBuilder.Entity<Order>()
                .Property(o => o.PaymentType)
                .HasConversion<int>();
                
            modelBuilder.Entity<Order>()
                .Property(o => o.PaymentStatus)
                .HasConversion<int>();

            // Configure Order-OrderItems relationship
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Order-OrderAddresses relationship (if using OrderAddress)
            modelBuilder.Entity<OrderAddress>()
                .HasOne(oa => oa.Order)
                .WithMany() // If Order doesn't have ICollection<OrderAddress>
                .HasForeignKey(oa => oa.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure OrderItem-Product relationship
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany() // Product doesn't need ICollection<OrderItem>
                .HasForeignKey(oi => oi.ProductId);

            // Configure enum for OrderAddress (if using)
            modelBuilder.Entity<OrderAddress>()
                .Property(oa => oa.AddressType)
                .HasConversion<int>();
            
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.CashOnDelivery)
                .WithOne(cod => cod.Transaction)
                .HasForeignKey<CashOnDelivery>(cod => cod.Id)
                .OnDelete(DeleteBehavior.Cascade); 

        }
    }
}