using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Data;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;

namespace MobileShopAPI.Repositories
{
    public class AttributeTypeRepository : IAttributeTypeRepository
    {
        private readonly AppDbContext _context;

        public AttributeTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AttributeType>> GetAllAsync() =>
            await _context.AttributeTypes.ToListAsync();

        public async Task<AttributeType?> GetByIdAsync(int id) =>
            await _context.AttributeTypes.FirstOrDefaultAsync(t => t.Id == id);

        public async Task AddAsync(AttributeType type) =>
            await _context.AttributeTypes.AddAsync(type);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public async Task<AttributeType?> GetByNameAsync(string name)
        {
            return await _context.AttributeTypes
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }
        
        public async Task UpdateAsync(AttributeType attributeType)
        {
            _context.AttributeTypes.Update(attributeType);
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteAsync(AttributeType attributeType) =>
            _context.AttributeTypes.Remove(attributeType);

        
    }
}
