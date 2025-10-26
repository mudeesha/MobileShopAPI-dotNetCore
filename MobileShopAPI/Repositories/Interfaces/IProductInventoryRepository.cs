using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface IProductInventoryRepository
    {
        Task<List<ProductInventory>> GetByProductIdAsync(int productId);
        Task<ProductInventory?> GetExactMatchAsync(int productId, List<int> attributeValueIds);
        Task AddAsync(ProductInventory inventory);
        Task SaveChangesAsync();
    }
}
