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

        public async Task AddRangeAsync(IEnumerable<AttributeValue> values)
        {
            await _context.AttributeValues.AddRangeAsync(values);
        }

        public async Task<List<AttributeValue>> GetAllByTypeIdAsync(int typeId)
        {
            return await _context.AttributeValues
                .Where(v => v.AttributeTypeId == typeId)
                .ToListAsync();
        }

        public async Task<AttributeValue?> GetByTypeAndValueAsync(int attributeTypeId, string value)
        {
            return await _context.AttributeValues
                .FirstOrDefaultAsync(av => av.AttributeTypeId == attributeTypeId &&
                                           av.Value.ToLower() == value.ToLower());
        }

        public async Task<List<AttributeValue>> GetByAttributeTypeIdAsync(int attributeTypeId)
        {
            return await _context.AttributeValues
                .Where(v => v.AttributeTypeId == attributeTypeId)
                .ToListAsync();
        }
        
        public async Task DeleteAsync(AttributeValue attributeValue)
        {
            _context.AttributeValues.Remove(attributeValue);
            await Task.CompletedTask;
        }



    }
}
