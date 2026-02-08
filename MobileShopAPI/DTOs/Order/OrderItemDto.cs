using System.ComponentModel.DataAnnotations;

namespace MobileShopAPI.DTOs.Order;

public class OrderItemDto
{
    public int Id { get; set; }
        
    [Required]
    public int ProductId { get; set; }
        
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    public string ProductName { get; set; } = null!;
    public string? ProductImage { get; set; }
    public string SKU { get; set; } = null!;
    public string? AttributeSummary { get; set; }
    
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}