using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Data;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;

namespace MobileShopAPI.Repositories
{
    public class OrderAddressRepository : IOrderAddressRepository
    {
        private readonly AppDbContext _context;

        public OrderAddressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderAddress?> GetByIdAsync(int id)
        {
            return await _context.OrderAddresses.FindAsync(id);
        }

        public async Task<IEnumerable<OrderAddress>> GetByOrderIdAsync(int orderId)
        {
            
             return await _context.OrderAddresses
                 .Where(oa => oa.OrderId == orderId)
                 .ToListAsync();
        }

        public async Task<OrderAddress?> GetByOrderAndTypeAsync(int orderId, Enums.AddressType type)
        {
            return await _context.OrderAddresses
                .FirstOrDefaultAsync(oa => oa.OrderId == orderId && oa.AddressType == type);
        }

        public async Task<OrderAddress> CreateAsync(OrderAddress address)
        {
            _context.OrderAddresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task UpdateAsync(OrderAddress address)
        {
            _context.Entry(address).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var address = await _context.OrderAddresses.FindAsync(id);
            if (address != null)
            {
                _context.OrderAddresses.Remove(address);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.OrderAddresses.AnyAsync(oa => oa.Id == id);
        }
        
        public async Task<IEnumerable<OrderAddress>> GetByOrderIdsAsync(List<int> orderIds)
        {
            return await _context.OrderAddresses
                .Where(oa => orderIds.Contains(oa.OrderId))
                .ToListAsync();
        }
        
    }
}