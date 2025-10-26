using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Data;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;

namespace MobileShopAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Model)
                    .ThenInclude(m => m.Brand)
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.AttributeValue)
                    .ThenInclude(av => av.AttributeType)
                .Include(p => p.ProductImages)
                .Include(p => p.InventoryAttributeValues)
                    .ThenInclude(iav => iav.AttributeValue)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Model)
                    .ThenInclude(m => m.Brand)
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.AttributeValue)
                .Include(p => p.ProductImages)
                .ToListAsync();
        }

        public IQueryable<Product> Query()
        {
            return _context.Products
                .Include(p => p.Model)
                    .ThenInclude(m => m.Brand)
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.AttributeValue)
                    .ThenInclude(av => av.AttributeType)
                .Include(p => p.ProductImages)
                .Include(p => p.InventoryAttributeValues)
                    .ThenInclude(iav => iav.AttributeValue)
                .AsQueryable();
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetByModelIdAsync(int modelId)
        {
            return await _context.Products
                .Where(p => p.ModelId == modelId)
                .Include(p => p.Model)
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.AttributeValue)
                .ToListAsync();
        }

        public async Task<Product?> GetBySkuAsync(string sku)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.SKU == sku);
        }
    }
}