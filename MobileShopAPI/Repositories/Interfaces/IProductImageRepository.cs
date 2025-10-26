using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface IProductImageRepository
    {
        Task<ProductImage?> GetByIdAsync(int id);
        Task<List<ProductImage>> GetAllAsync();
        Task AddAsync(ProductImage productImage);
        Task UpdateAsync(ProductImage productImage);
        Task DeleteAsync(ProductImage productImage);
        Task SaveChangesAsync();
        
        // New methods for image assignments
        Task AssignImageToProductAsync(int productId, int productImageId, bool isDefault = false);
        Task RemoveImageFromProductAsync(int productId, int productImageId);
        Task<List<ProductImage>> GetImagesByProductAsync(int productId);
        Task<List<Product>> GetProductsByImageAsync(int productImageId);
        Task SetDefaultImageAsync(int productId, int productImageId);
    }
}