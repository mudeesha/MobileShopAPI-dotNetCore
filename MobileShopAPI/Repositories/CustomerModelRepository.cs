// Repositories/CustomerModelRepository.cs
using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Data;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;

namespace MobileShopAPI.Repositories
{
    public class CustomerModelRepository : ICustomerModelRepository
    {
        private readonly AppDbContext _context;

        public CustomerModelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Model>> GetModelsWithDetailsAsync()
        {
            return await _context.Models
                .AsNoTracking()
                .Include(m => m.Brand)
                .Include(m => m.Products)
                    .ThenInclude(p => p.ProductImageAssignments)
                        .ThenInclude(pia => pia.ProductImage)
                .Include(m => m.Products)
                    .ThenInclude(p => p.ProductAttributes)
                        .ThenInclude(pa => pa.AttributeValue)
                            .ThenInclude(av => av.AttributeType)
                .Where(m => m.Products.Any(p => p.StockQuantity > 0))
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByAttributeValueIdsAsync(int modelId, List<int> attributeValueIds)
        {
            // Get count of attribute values needed
            var attributeCount = attributeValueIds.Count;
            
            // Query optimization: Get all products for model and filter in memory
            var products = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductImageAssignments)
                    .ThenInclude(pia => pia.ProductImage)
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.AttributeValue)
                        .ThenInclude(av => av.AttributeType)
                .Where(p => p.ModelId == modelId && p.StockQuantity > 0)
                .ToListAsync();

            // Find product that matches exactly the attribute value IDs
            foreach (var product in products)
            {
                if (product.ProductAttributes == null) 
                    continue;
                
                // Get product's attribute value IDs
                var productAttrValueIds = product.ProductAttributes
                    .Where(pa => pa.AttributeValue != null)
                    .Select(pa => pa.AttributeValue!.Id)
                    .ToList();
                
                // Check for exact match
                if (productAttrValueIds.Count == attributeCount &&
                    attributeValueIds.All(id => productAttrValueIds.Contains(id)))
                {
                    return product;
                }
            }
            
            return null;
        }

        // Optional: Add a more optimized query using raw SQL for better performance
        public async Task<Product?> GetProductByAttributeValueIdsOptimizedAsync(int modelId, List<int> attributeValueIds)
        {
            var attributeCount = attributeValueIds.Count;
            var idsString = string.Join(",", attributeValueIds);
            
            // Using raw SQL for better performance with complex filtering
            var query = @"
                SELECT p.*
                FROM Products p
                WHERE p.ModelId = {0} AND p.StockQuantity > 0
                AND EXISTS (
                    SELECT 1
                    FROM ProductAttributes pa
                    WHERE pa.ProductId = p.Id
                    AND pa.AttributeValueId IN ({1})
                    GROUP BY pa.ProductId
                    HAVING COUNT(DISTINCT pa.AttributeValueId) = {2}
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM ProductAttributes pa2
                    WHERE pa2.ProductId = p.Id
                    AND pa2.AttributeValueId NOT IN ({1})
                )";
            
            var product = await _context.Products
                .FromSqlRaw(query, modelId, idsString, attributeCount)
                .Include(p => p.ProductImageAssignments)
                    .ThenInclude(pia => pia.ProductImage)
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.AttributeValue)
                        .ThenInclude(av => av.AttributeType)
                .FirstOrDefaultAsync();
            
            return product;
        }
    }
}