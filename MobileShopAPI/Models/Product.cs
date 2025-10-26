using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileShopAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ModelId { get; set; }

        [Required]
        public string SKU { get; set; } = null!;

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Navigation properties
        [ForeignKey("ModelId")]
        public virtual Model? Model { get; set; }

        // Product Attributes (many-to-many with AttributeValue)
        public virtual ICollection<ProductAttribute>? ProductAttributes { get; set; }

        // Product Images
        public virtual ICollection<ProductImage>? ProductImages { get; set; }

        // Inventory Attribute Values (for variants)
        public virtual ICollection<InventoryAttributeValue>? InventoryAttributeValues { get; set; }

        // Timestamps (optional - add if needed)
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}