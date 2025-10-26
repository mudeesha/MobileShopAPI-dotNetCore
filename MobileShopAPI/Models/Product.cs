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
        
        [ForeignKey("ModelId")]
        public virtual Model? Model { get; set; }
        
        public virtual ICollection<ProductAttribute>? ProductAttributes { get; set; }
        
        public virtual ICollection<ProductImageAssignment>? ProductImageAssignments { get; set; }
        
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}