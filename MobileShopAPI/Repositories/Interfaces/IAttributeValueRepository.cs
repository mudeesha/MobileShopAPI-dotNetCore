using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface IAttributeValueRepository
    {
        Task<List<AttributeValue>> GetAllAsync();
        Task<List<AttributeValue>> GetByIdsAsync(List<int> ids);
        Task AddAsync(AttributeValue value);
        Task SaveChangesAsync();
    }
}
