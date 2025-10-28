using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface IModelRepository
    {
        Task<List<Model>> GetAllAsync();
        Task<Model?> GetByIdAsync(int id);
        Task AddAsync(Model model);
        Task DeleteAsync(Model model);
        Task SaveChangesAsync();
        Task<List<Model>> GetAllWithProductsAsync();
        Task UpdateAsync(Model model);
    }
}
