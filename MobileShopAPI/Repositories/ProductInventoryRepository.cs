using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Data;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;

namespace MobileShopAPI.Repositories
{
    public class ProductInventoryRepository : IProductInventoryRepository
    {
        private readonly AppDbContext _context;

        public ProductInventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductInventory>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductInventories
                .Include(pi => pi.InventoryAttributeValues)
                    .ThenInclude(iav => iav.AttributeValue)
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();
        }

        public async Task<ProductInventory?> GetExactMatchAsync(int productId, List<int> attributeValueIds)
        {
            // Bring matching inventories into memory first
            var inventories = await _context.ProductInventories
                .Include(pi => pi.InventoryAttributeValues)
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();  // EF translates up to here, then LINQ-to-Objects takes over

            // Then do sequence comparison in memory
            return inventories.FirstOrDefault(pi =>
                pi.InventoryAttributeValues
                  .Select(v => v.AttributeValueId)
                  .OrderBy(id => id)
                  .SequenceEqual(attributeValueIds.OrderBy(id => id)));
        }


        public async Task AddAsync(ProductInventory inventory)
        {
            await _context.ProductInventories.AddAsync(inventory);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
