using System.ComponentModel.DataAnnotations.Schema;

namespace MobileShopAPI.Models
{
    public class ProductAttribute
    {
        public int ProductId { get; set; }
        public int AttributeValueId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("AttributeValueId")]
        public virtual AttributeValue? AttributeValue { get; set; }
    }
}

