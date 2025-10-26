using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Data;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;

namespace MobileShopAPI.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _context;

        public ProductImageRepository(AppDbContext context)
        {
            _context = context; 
        }

        public async Task<List<ProductImage>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();
        }

        public async Task AddAsync(ProductImage image)
        {
            await _context.ProductImages.AddAsync(image);
        }

        public async Task DeleteAsync(ProductImage image)
        {
            _context.ProductImages.Remove(image);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
