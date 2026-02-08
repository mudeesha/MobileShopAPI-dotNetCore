using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(int id);
    Task<Transaction?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Transaction>> GetByOrderIdAsync(int orderId);
    Task<IEnumerable<Transaction>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction> CreateAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
}