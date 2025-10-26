using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface IProductRepository
    {
        IQueryable<Product> Query();
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task DeleteAsync(Product product);
        Task SaveChangesAsync();
        Task<int> GetStockByAttributesAsync(int productId, List<int> attributeValueIds);

    }
}
