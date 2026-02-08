using MobileShopAPI.DTOs.Transaction;
using MobileShopAPI.Models;

namespace MobileShopAPI.Services.Interfaces;

public interface ITransactionService
{
    Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto, string userId);
    Task<TransactionDto> GetTransactionAsync(int transactionId, string userId);
    Task<List<TransactionDto>> GetUserTransactionsAsync(string userId);
    Task<TransactionDto> UpdateCashCollectionAsync(int transactionId, UpdateCashCollectionDto dto, string adminId);
    Task<List<TransactionDto>> GetAllTransactionsAsync(Enums.TransactionStatus? status = null);
}