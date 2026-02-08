using System.ComponentModel.DataAnnotations;
using MobileShopAPI.Models;

namespace MobileShopAPI.DTOs.Transaction;

public class CreateTransactionDto
{
    [Required]
    public int OrderId { get; set; }
        
    [Required]
    public Enums.PaymentMethod PaymentMethod { get; set; }
        
    [MaxLength(10)]
    public string Currency { get; set; } = "LKR";
        
    [MaxLength(500)]
    public string? Notes { get; set; }
        
    // Cash on Delivery specific
    public CreateCashOnDeliveryDto? CashOnDelivery { get; set; }
}