using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileShopAPI.Models
{
    public class InventoryAttributeValue
    {
        [Required]
        public int ProductInventoryId { get; set; }

        [Required]
        public int AttributeValueId { get; set; }

        // Navigation
        public ProductInventory ProductInventory { get; set; } = null!;
        public AttributeValue AttributeValue { get; set; } = null!;
    }
}
