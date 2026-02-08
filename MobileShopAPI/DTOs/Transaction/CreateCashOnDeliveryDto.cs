using System.ComponentModel.DataAnnotations;
namespace MobileShopAPI.DTOs.Transaction;

public class CreateCashOnDeliveryDto
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal CollectedAmount { get; set; }
        
    [MaxLength(255)]
    public string? DeliveryPersonName { get; set; }
        
    [MaxLength(20)]
    [Phone]
    public string? DeliveryPersonPhone { get; set; }
}