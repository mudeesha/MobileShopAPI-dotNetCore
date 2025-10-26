using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetAllAsync();
        IQueryable<Product> Query();
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task SaveChangesAsync();
        
        // Add any specific product queries you need
        Task<List<Product>> GetByModelIdAsync(int modelId);
        Task<Product?> GetBySkuAsync(string sku);
    }
}