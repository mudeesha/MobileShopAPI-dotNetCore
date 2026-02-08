using MobileShopAPI.DTOs.Order;
using MobileShopAPI.Models;

namespace MobileShopAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto);
        Task<List<OrderDto>> GetUserOrdersAsync(string userId);
        Task<OrderDto> GetOrderByIdAsync(int orderId, string userId);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto, string adminId);
        Task<bool> CancelOrderAsync(int orderId, string userId);
        
        Task<List<OrderSummaryDto>> GetAllOrdersAsync(Enums.OrderStatus? status = null);
        Task<OrderDto> GetOrderForAdminAsync(int orderId);
    }
}