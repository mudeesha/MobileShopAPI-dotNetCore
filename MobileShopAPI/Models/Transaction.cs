namespace MobileShopAPI.Models;

public class Transaction
{
    
    public int Id { get; set; }
    public string TransactionNumber { get; set; }
    public int OrderId { get; set; }
    
    public Enums.PaymentMethod PaymentType { get; set; }
    public Enums.TransactionStatus Status { get; set; }
    
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "LKR";
    public string Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual Order Order { get; set; }
    
    public virtual CashOnDelivery CashOnDelivery { get; set; }
    public virtual OnlinePayment OnlinePayment { get; set; }
    public virtual BankTransfer BankTransfer { get; set; }
    
}