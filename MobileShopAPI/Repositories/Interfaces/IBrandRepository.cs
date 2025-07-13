using MobileShopAPI.Models;
using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface IBrandRepository
    {
        Task<List<Brand>> GetAllAsync();
        Task<Brand?> GetByIdAsync(int id);
        Task AddAsync(Brand brand);
        Task DeleteAsync(Brand brand);
        Task SaveChangesAsync();
    }
}
