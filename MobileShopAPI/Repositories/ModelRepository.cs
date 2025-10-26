using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Data;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;

namespace MobileShopAPI.Repositories
{
    public class ModelRepository : IModelRepository
    {
        private readonly AppDbContext _context;

        public ModelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Model>> GetAllAsync() =>
            await _context.Models.Include(m => m.Brand).ToListAsync();

        public async Task<Model?> GetByIdAsync(int id) =>
            await _context.Models.Include(m => m.Brand)
                                 .FirstOrDefaultAsync(m => m.Id == id);

        public async Task AddAsync(Model model) =>
            await _context.Models.AddAsync(model);

        public async Task DeleteAsync(Model model) =>
            _context.Models.Remove(model);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public async Task<List<Model>> GetAllWithProductsAsync()
        {
            return await _context.Models
                .Include(m => m.Brand)
                .Include(m => m.Products)
                .ThenInclude(p => p.ProductAttributes)
                .ThenInclude(pa => pa.AttributeValue)
                .Include(m => m.Products)
                .ThenInclude(p => p.ProductImages)
                .ToListAsync();
        }


    }
}
