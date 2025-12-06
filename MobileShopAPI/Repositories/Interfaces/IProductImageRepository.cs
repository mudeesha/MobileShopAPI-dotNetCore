// IProductImageRepository
using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface IProductImageRepository
    {
        // ✅ Added GetByIdAsync method
        Task<ProductImage?> GetByIdAsync(int id);
        
        Task<List<ProductImage>> GetAllAsync(int pageNumber = 1, int pageSize = 20);
        Task<int> GetTotalCountAsync();
        Task<List<ProductImage>> GetAllAsync(); // ✅ This exists in interface
        Task AddAsync(ProductImage productImage);
        Task UpdateAsync(ProductImage productImage);
        Task DeleteAsync(ProductImage productImage);
        Task SaveChangesAsync();
        
        // Assignment operations
        Task AssignImageToProductAsync(int productId, int productImageId, bool isDefault = false);
        Task RemoveImageFromProductAsync(int productId, int productImageId);
        Task<List<ProductImage>> GetImagesByProductAsync(int productId);
        Task<List<Product>> GetProductsByImageAsync(int productImageId);
        Task SetDefaultImageAsync(int productId, int productImageId);
    }
}