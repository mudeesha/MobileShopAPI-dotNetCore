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
    
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    
    public Enums.PaymentMethod PaymentMethod { get; set; }
    public Enums.PaymentStatus PaymentStatus { get; set; }
    
    public AddressDto ShippingAddress { get; set; } = null!;
    public AddressDto? BillingAddress { get; set; }
    
    public List<OrderItemDto> OrderItems { get; set; } = new();
    
    public string? TrackingNumber { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    
    public string? CustomerNotes { get; set; }
    public string? AdminNotes { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}