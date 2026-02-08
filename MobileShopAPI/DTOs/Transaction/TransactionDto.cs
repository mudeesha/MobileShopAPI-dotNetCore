using MobileShopAPI.Models;

namespace MobileShopAPI.DTOs.Transaction;

public class TransactionDto
{
    public int Id { get; set; }
    public string TransactionNumber { get; set; } = null!;
    public int OrderId { get; set; }
    public Enums.PaymentMethod PaymentMethod { get; set; }
    public Enums.TransactionStatus Status { get; set; }
        
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "LKR";
        
    public string? Notes { get; set; }
        
    // Payment type specific data
    public CashOnDeliveryDto? CashOnDelivery { get; set; }
        
    // Order info (optional)
    public string? OrderNumber { get; set; }
    public decimal? OrderTotal { get; set; }
        
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}