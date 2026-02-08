using System.ComponentModel.DataAnnotations;
using MobileShopAPI.Models;

namespace MobileShopAPI.DTOs.Order;

public class UpdateOrderStatusDto
{
    [Required]
    public Enums.OrderStatus Status { get; set; }
        
    [MaxLength(1000)]
    public string? AdminNotes { get; set; }
        
    [MaxLength(100)]
    public string? TrackingNumber { get; set; }
}