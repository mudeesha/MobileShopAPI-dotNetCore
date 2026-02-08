using MobileShopAPI.Models;
using MobileShopAPI.DTOs;
using MobileShopAPI.Models;

namespace MobileShopAPI.DTOs.Order;

public class OrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public Enums.OrderStatus Status { get; set; }
        
    // Amounts
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
        
    // Payment
    public Enums.PaymentMethod PaymentMethod { get; set; }
    public Enums.PaymentStatus PaymentStatus { get; set; }
        
    // Addresses
    public AddressDto ShippingAddress { get; set; } = null!;
    public AddressDto? BillingAddress { get; set; }
        
    // Order Items
    public List<OrderItemDto> OrderItems { get; set; } = new();
        
    // Tracking
    public string? TrackingNumber { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
        
    // Notes
    public string? CustomerNotes { get; set; }
    public string? AdminNotes { get; set; }
        
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}