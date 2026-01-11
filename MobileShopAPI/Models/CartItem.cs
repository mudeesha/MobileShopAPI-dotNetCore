using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileShopAPI.Models;

public class CartItem
{
    [Key]
    public int Id { get; set; }
        
    [Required]
    public int CartId { get; set; }
         
    [Required]
    public int ProductId { get; set; }
        
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
        
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
        
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
    [ForeignKey("CartId")]
    public virtual Cart? Cart { get; set; }
        
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}