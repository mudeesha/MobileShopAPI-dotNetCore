using System.ComponentModel.DataAnnotations.Schema;

namespace MobileShopAPI.Models
{
    public class ProductImageAssignment
    {
        public int ProductId { get; set; }
        public int ProductImageId { get; set; }
        public bool IsDefault { get; set; }
        
        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("ProductImageId")]
        public virtual ProductImage? ProductImage { get; set; }
    }
}