using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Data;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;

namespace MobileShopAPI.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;

        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Brand>> GetAllAsync() =>
            await _context.Brands.Include(b => b.Models).ToListAsync();

        public async Task<Brand?> GetByIdAsync(int id) =>
            await _context.Brands.Include(b => b.Models).FirstOrDefaultAsync(b => b.Id == id);

        public async Task AddAsync(Brand brand) =>
            await _context.Brands.AddAsync(brand);

        public async Task DeleteAsync(Brand brand) =>
            _context.Brands.Remove(brand);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
