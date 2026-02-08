namespace MobileShopAPI.Models;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public Enums.OrderStatus Status { get; set; }
    
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    
    public Enums.PaymentMethod PaymentType { get; set; }
    public Enums.PaymentStatus PaymentStatus { get; set; }
    public string? CustomerNotes { get; set; }
    public string? AdminNotes { get; set; }
    public string? TrackingNumber { get; set; }
    
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public virtual ICollection<OrderAddress> Addresses { get; set; } = new List<OrderAddress>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}