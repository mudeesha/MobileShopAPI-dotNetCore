using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces
{
    public interface ICashOnDeliveryRepository
    {
        Task<CashOnDelivery> CreateAsync(CashOnDelivery cashOnDelivery);
        Task<CashOnDelivery?> GetByTransactionIdAsync(int transactionId);
        Task UpdateAsync(CashOnDelivery cashOnDelivery);
        Task DeleteAsync(int id);
    }
}