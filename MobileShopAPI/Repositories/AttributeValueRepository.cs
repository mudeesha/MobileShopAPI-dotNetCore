using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Data;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;

namespace MobileShopAPI.Repositories
{
    public class AttributeValueRepository : IAttributeValueRepository
    {
        private readonly AppDbContext _context;

        public AttributeValueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AttributeValue>> GetAllAsync()
        {
            return await _context.AttributeValues
                .Include(av => av.AttributeType)
                .ToListAsync();
        }

        public async Task<List<AttributeValue>> GetByIdsAsync(List<int> ids)
        {
            return await _context.AttributeValues
                .Include(av => av.AttributeType)
                .Where(av => ids.Contains(av.Id))
                .ToListAsync();
        }

        public async Task AddAsync(AttributeValue value)
        {
            await _context.AttributeValues.AddAsync(value);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
