using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface IAttributeTypeRepository
    {
        Task<List<AttributeType>> GetAllAsync();
        Task<AttributeType?> GetByIdAsync(int id);
        Task AddAsync(AttributeType type);
        Task SaveChangesAsync();
        Task<AttributeType?> GetByNameAsync(string name);

    }
}
