using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileShopAPI.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public string ImageUrl { get; set; } = null!;
        public bool IsDefault { get; set; }

        public virtual ICollection<ProductImageAttributeValue>? ProductImageAttributeValues { get; set; }
    }
}
