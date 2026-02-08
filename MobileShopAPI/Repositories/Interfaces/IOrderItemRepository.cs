using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces;

public interface IOrderItemRepository
{
    Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
    Task<OrderItem> CreateAsync(OrderItem orderItem);
    Task UpdateAsync(OrderItem orderItem);
    Task DeleteAsync(int id);
    Task<Dictionary<int, int>> GetItemCountsByOrderIdsAsync(List<int> orderIds);
    Task<IEnumerable<OrderItem>> GetByOrderIdsAsync(List<int> orderIds);
    
}