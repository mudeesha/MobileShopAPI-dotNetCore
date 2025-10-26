using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileShopAPI.Models
{
    public class InventoryAttributeValue
    {
        public int ProductId { get; set; } // This should exist now
        public int AttributeValueId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("AttributeValueId")]
        public virtual AttributeValue? AttributeValue { get; set; }
    }
}
