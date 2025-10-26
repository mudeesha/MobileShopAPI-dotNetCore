using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface IProductImageRepository
    {
        Task<List<ProductImage>> GetByProductIdAsync(int productId);
        Task AddAsync(ProductImage image);
        Task DeleteAsync(ProductImage image);
        Task SaveChangesAsync();
    }
}
