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

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Model)
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.AttributeValue)
                        .ThenInclude(av => av.AttributeType)
                .Include(p => p.ProductInventories)
                .Include(p => p.ProductImages)
                    .ThenInclude(pi => pi.ProductImageAttributeValues)
                .ToListAsync();
        }

        public IQueryable<Product> Query()
        {
            return _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Model)
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.AttributeValue)
                        .ThenInclude(av => av.AttributeType)
                .Include(p => p.ProductInventories)
                .Include(p => p.ProductImages)
                    .ThenInclude(pi => pi.ProductImageAttributeValues)
                .AsQueryable();
        }

        public async Task<Product?> GetByIdAsync(int id) =>
            await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Model)
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.AttributeValue)
                        .ThenInclude(av => av.AttributeType)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task AddAsync(Product product) =>
            await _context.Products.AddAsync(product);

        public async Task DeleteAsync(Product product) =>
            _context.Products.Remove(product);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public async Task<int> GetStockByAttributesAsync(int productId, List<int> attributeValueIds)
        {
            var inventory = await _context.ProductInventories
                .Include(pi => pi.InventoryAttributeValues)
                .FirstOrDefaultAsync(pi =>
                    pi.ProductId == productId &&
                    pi.InventoryAttributeValues.Select(v => v.AttributeValueId).OrderBy(id => id)
                        .SequenceEqual(attributeValueIds.OrderBy(id => id)));

            return inventory?.StockQuantity ?? 0;
        }

    }
}
