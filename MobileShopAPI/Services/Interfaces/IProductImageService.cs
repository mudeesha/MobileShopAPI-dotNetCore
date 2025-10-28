using MobileShopAPI.DTOs;

namespace MobileShopAPI.Services.Interfaces
{
    public interface IProductImageService
    {
        Task<ProductImageDto> CreateImageAsync(ProductImageCreateDto dto);
        Task<bool> UpdateImageAsync(int id, ProductImageUpdateDto dto); // ✅ ADD THIS
        Task<bool> AssignImageToProductAsync(ImageAssignmentDto dto);
        Task<bool> RemoveImageFromProductAsync(int productId, int productImageId);
        Task<List<ProductImageDto>> GetImagesByProductAsync(int productId);
        Task<List<ProductDto>> GetProductsByImageAsync(int productImageId);
        Task<bool> SetDefaultImageAsync(int productId, int productImageId);
        Task<bool> DeleteImageAsync(int id);
    }
}