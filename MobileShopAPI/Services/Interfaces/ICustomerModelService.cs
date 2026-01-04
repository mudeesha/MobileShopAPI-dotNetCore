// Services/Interfaces/ICustomerModelService.cs
using MobileShopAPI.DTOs;

namespace MobileShopAPI.Services.Interfaces
{
    public interface ICustomerModelService
    {
        Task<List<ModelListingDto>> GetModelListingAsync();
        Task<ProductVariantDto?> GetProductByAttributeValueIdsAsync(int modelId, List<int> attributeValueIds);
    }
}