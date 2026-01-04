using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface ICustomerModelRepository
    {
        Task<List<Model>> GetModelsWithDetailsAsync();
        Task<Product?> GetProductByAttributeValueIdsAsync(int modelId, List<int> attributeValueIds);
    }
}