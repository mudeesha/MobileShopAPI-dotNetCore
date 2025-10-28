using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface IAttributeValueRepository
    {
        Task<List<AttributeValue>> GetAllAsync();
        Task<List<AttributeValue>> GetByIdsAsync(List<int> ids);
        Task AddAsync(AttributeValue value);
        Task AddRangeAsync(IEnumerable<AttributeValue> values);
        Task DeleteAsync(AttributeValue attributeValue);
        Task SaveChangesAsync();
        Task<AttributeValue?> GetByTypeAndValueAsync(int attributeTypeId, string value);
        Task<List<AttributeValue>> GetByAttributeTypeIdAsync(int attributeTypeId);


    }
}
