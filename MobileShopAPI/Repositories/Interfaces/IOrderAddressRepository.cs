using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface IOrderAddressRepository
    {
        Task<OrderAddress?> GetByIdAsync(int id);
        Task<IEnumerable<OrderAddress>> GetByOrderIdAsync(int orderId);
        Task<OrderAddress?> GetByOrderAndTypeAsync(int orderId, Enums.AddressType type);
        Task<OrderAddress> CreateAsync(OrderAddress address);
        Task UpdateAsync(OrderAddress address);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<OrderAddress>> GetByOrderIdsAsync(List<int> orderIds);
        
    }
}