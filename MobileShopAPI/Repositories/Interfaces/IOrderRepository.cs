using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByIdWithItemsAsync(int id);
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByStatusAsync(Enums.OrderStatus status);
        Task<Order> CreateAsync(Order order);
        Task UpdateAsync(Order order);
        Task<bool> ExistsAsync(int id);
        Task<int> SaveChangesAsync();
    }
}