// Services/TransactionService.cs
using MobileShopAPI.DTOs.Transaction;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICashOnDeliveryRepository _cashOnDeliveryRepository;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IOrderRepository orderRepository,
            ICashOnDeliveryRepository cashOnDeliveryRepository,
            ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _orderRepository = orderRepository;
            _cashOnDeliveryRepository = cashOnDeliveryRepository;
            _logger = logger;
        }

        public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto dto, string userId)
        {
            try
            {
                // 1. Verify order exists and belongs to user
                var order = await _orderRepository.GetByIdAsync(dto.OrderId);
                if (order == null)
                    throw new KeyNotFoundException("Order not found");

                if (order.UserId != userId)
                    throw new UnauthorizedAccessException("You don't have permission to create transaction for this order");

                // 2. Check if transaction already exists for this order
                var existingTransactions = await _transactionRepository.GetByOrderIdAsync(dto.OrderId);
                if (existingTransactions.Any())
                    throw new InvalidOperationException("Transaction already exists for this order");

                // 3. Generate transaction number
                var transactionNumber = GenerateTransactionNumber();

                // 4. Create transaction
                var transaction = new Transaction
                {
                    TransactionNumber = transactionNumber,
                    OrderId = dto.OrderId,
                    PaymentType = dto.PaymentMethod,
                    Status = Enums.TransactionStatus.Pending,
                    Amount =  order.TotalAmount,
                    Currency = dto.Currency,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // 5. Save transaction
                await _transactionRepository.CreateAsync(transaction);

                // 6. Create CashOnDelivery record if needed
                if (dto.PaymentMethod == Enums.PaymentMethod.CashOnDelivery && dto.CashOnDelivery != null)
                {
                    var cashOnDelivery = new CashOnDelivery
                    {
                        Id = transaction.Id,
                        TransactionId = transaction.Id,
                        ExpectedAmount = order.TotalAmount,
                        DeliveryPersonName = dto.CashOnDelivery.DeliveryPersonName ?? string.Empty,
                        DeliveryPersonPhone = dto.CashOnDelivery.DeliveryPersonPhone ?? string.Empty,
                        
                        CollectedBy = "System",
                        CollectedAmount = dto.CashOnDelivery.CollectedAmount,
                        CollectedDate = DateTime.UtcNow,
                        CollectorNotes = string.Empty
                    };

                    await _cashOnDeliveryRepository.CreateAsync(cashOnDelivery);
                }

                // 7. Return created transaction
                return await GetTransactionAsync(transaction.Id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction for order {OrderId} by user {UserId}", 
                    dto.OrderId, userId);
                throw;
            }
        }

        public async Task<TransactionDto> GetTransactionAsync(int transactionId, string userId)
        {
            var transaction = await _transactionRepository.GetByIdWithDetailsAsync(transactionId);
            if (transaction == null)
                throw new KeyNotFoundException("Transaction not found");

            // Verify user has access to this transaction
            if (transaction.Order?.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to view this transaction");

            return MapToTransactionDto(transaction);
        }

        public async Task<List<TransactionDto>> GetUserTransactionsAsync(string userId)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            return transactions.Select(MapToTransactionDto).ToList();
        }

        public async Task<TransactionDto> UpdateCashCollectionAsync(int transactionId, UpdateCashCollectionDto dto, string adminId)
        {
            var transaction = await _transactionRepository.GetByIdWithDetailsAsync(transactionId);
            if (transaction == null)
                throw new KeyNotFoundException("Transaction not found");

            if (transaction.PaymentType != Enums.PaymentMethod.CashOnDelivery)
                throw new InvalidOperationException("Only Cash on Delivery transactions can be updated");

            var cashOnDelivery = await _cashOnDeliveryRepository.GetByTransactionIdAsync(transactionId);
            if (cashOnDelivery == null)
                throw new InvalidOperationException("Cash on Delivery record not found");

            // Update cash collection
            cashOnDelivery.CollectedAmount = dto.CollectedAmount;
            cashOnDelivery.CollectedDate = dto.CollectedDate;
            cashOnDelivery.CollectedBy = dto.CollectedBy;
            cashOnDelivery.CollectorNotes = dto.CollectorNotes;

            await _cashOnDeliveryRepository.UpdateAsync(cashOnDelivery);

            // Update transaction status
            transaction.Status = Enums.TransactionStatus.Completed;
            transaction.UpdatedAt = DateTime.UtcNow;
            await _transactionRepository.UpdateAsync(transaction);

            // Update order payment status
            var order = await _orderRepository.GetByIdAsync(transaction.OrderId);
            if (order != null)
            {
                order.PaymentStatus = Enums.PaymentStatus.Paid;
                await _orderRepository.UpdateAsync(order);
            }

            _logger.LogInformation("Cash collected for transaction {TransactionId} by {AdminId}", 
                transactionId, adminId);

            return await GetTransactionAsync(transactionId, order?.UserId ?? adminId);
        }

        public async Task<List<TransactionDto>> GetAllTransactionsAsync(Enums.TransactionStatus? status = null)
        {
            var transactions = await _transactionRepository.GetAllAsync();
            
            if (status.HasValue)
                transactions = transactions.Where(t => t.Status == status.Value).ToList();

            return transactions.Select(MapToTransactionDto).ToList();
        }

        // Helper methods
        private string GenerateTransactionNumber()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = new Random().Next(10000, 99999);
            return $"TRX-{datePart}-{randomPart}";
        }

        private TransactionDto MapToTransactionDto(Transaction transaction)
        {
            var dto = new TransactionDto
            {
                Id = transaction.Id,
                TransactionNumber = transaction.TransactionNumber,
                OrderId = transaction.OrderId,
                PaymentMethod = transaction.PaymentType,
                Status = transaction.Status,
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                Notes = transaction.Notes,
                CreatedAt = transaction.CreatedAt,
                UpdatedAt = transaction.UpdatedAt
            };

            // Add order info
            if (transaction.Order != null)
            {
                dto.OrderNumber = transaction.Order.OrderNumber;
                dto.OrderTotal = transaction.Order.TotalAmount;
            }

            // Add CashOnDelivery data if exists
            if (transaction.CashOnDelivery != null)
            {
                dto.CashOnDelivery = new CashOnDeliveryDto
                {
                    Id = transaction.CashOnDelivery.Id,
                    TransactionId = transaction.CashOnDelivery.TransactionId,
                    ExpectedAmount = transaction.CashOnDelivery.ExpectedAmount,
                    CollectedAmount = transaction.CashOnDelivery.CollectedAmount,
                    CollectedDate = transaction.CashOnDelivery.CollectedDate,
                    CollectedBy = transaction.CashOnDelivery.CollectedBy,
                    CollectorNotes = transaction.CashOnDelivery.CollectorNotes,
                    DeliveryPersonName = transaction.CashOnDelivery.DeliveryPersonName,
                    DeliveryPersonPhone = transaction.CashOnDelivery.DeliveryPersonPhone
                };
            }
            else
            {
                // Try to load CashOnDelivery if not included
                var cashOnDelivery = _cashOnDeliveryRepository.GetByTransactionIdAsync(transaction.Id).Result;
                if (cashOnDelivery != null)
                {
                    dto.CashOnDelivery = new CashOnDeliveryDto
                    {
                        Id = cashOnDelivery.Id,
                        TransactionId = cashOnDelivery.TransactionId,
                        ExpectedAmount = cashOnDelivery.ExpectedAmount,
                        CollectedAmount = cashOnDelivery.CollectedAmount,
                        CollectedDate = cashOnDelivery.CollectedDate,
                        CollectedBy = cashOnDelivery.CollectedBy,
                        CollectorNotes = cashOnDelivery.CollectorNotes,
                        DeliveryPersonName = cashOnDelivery.DeliveryPersonName,
                        DeliveryPersonPhone = cashOnDelivery.DeliveryPersonPhone
                    };
                }
            }

            return dto;
        }
    }
}