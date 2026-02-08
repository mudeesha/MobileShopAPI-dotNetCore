using System.ComponentModel.DataAnnotations;
using MobileShopAPI.Models;

namespace MobileShopAPI.DTOs.Order;

public class CartCheckoutDto
{
    [Required]
    public AddressDto ShippingAddress { get; set; } = null!;
        
    public AddressDto? BillingAddress { get; set; }
        
    [Required]
    public Enums.PaymentMethod PaymentMethod { get; set; }
        = Enums.PaymentMethod.CashOnDelivery;
        
    [MaxLength(1000)]
    public string? CustomerNotes { get; set; }
    
    public int? ShippingMethodId { get; set; }
    
    public string? DiscountCode { get; set; }
}