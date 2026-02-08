using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Data;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;

namespace MobileShopAPI.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.Product)
                .ThenInclude(p => p.Model)
                .Include(oi => oi.Product.ProductImageAssignments)
                .ThenInclude(pia => pia.ProductImage)
                .ToListAsync();
        }

        public async Task<OrderItem> CreateAsync(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
            return orderItem;
        }

        public async Task UpdateAsync(OrderItem orderItem)
        {
            _context.Entry(orderItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task<Dictionary<int, int>> GetItemCountsByOrderIdsAsync(List<int> orderIds)
        {
            return await _context.OrderItems
                .Where(oi => orderIds.Contains(oi.OrderId))
                .GroupBy(oi => oi.OrderId)
                .Select(g => new { OrderId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.OrderId, x => x.Count);
        }
        
        public async Task<IEnumerable<OrderItem>> GetByOrderIdsAsync(List<int> orderIds)
        {
            return await _context.OrderItems
                .Where(oi => orderIds.Contains(oi.OrderId))
                .Include(oi => oi.Product)
                .ThenInclude(p => p.Model)
                .Include(oi => oi.Product.ProductImageAssignments)
                .ThenInclude(pia => pia.ProductImage)
                .Include(oi => oi.Product.ProductAttributes)
                .ThenInclude(pa => pa.AttributeValue)
                .ThenInclude(av => av.AttributeType)
                .ToListAsync();
        }
        
        
    }
}