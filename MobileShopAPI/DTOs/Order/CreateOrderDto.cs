using System.ComponentModel.DataAnnotations;
using MobileShopAPI.DTOs;
using MobileShopAPI.Models;

namespace MobileShopAPI.DTOs.Order;

public class CreateOrderDto
{
    [Required]
    public AddressDto ShippingAddress { get; set; } = null!;

    [Required]
    public Enums.PaymentMethod PaymentMethod { get; set; }
        = Enums.PaymentMethod.CashOnDelivery;

    [MaxLength(1000)]
    public string? CustomerNotes { get; set; }
}