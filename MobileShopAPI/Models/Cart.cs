using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileShopAPI.Models;

public class Cart
{
    [Key]
    public int Id { get; set; }
        
    [Required]
    public string UserId { get; set; } = string.Empty;
        
    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }
        
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}