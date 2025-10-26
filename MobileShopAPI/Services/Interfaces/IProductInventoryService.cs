using MobileShopAPI.DTOs;

namespace MobileShopAPI.Services.Interfaces
{
    public interface IProductInventoryService
    {
        Task<List<ProductInventoryDto>> GetByProductIdAsync(int productId);
        Task<ProductInventoryDto?> GetExactMatchAsync(int productId, List<int> attributeValueIds);
        Task<ProductInventoryDto> CreateAsync(ProductInventoryCreateDto dto);
    }
}

