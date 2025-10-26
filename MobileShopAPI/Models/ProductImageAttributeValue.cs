using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileShopAPI.Models
{
    public class ProductImageAttributeValue
    {
        public int ProductImageId { get; set; }
        public int AttributeValueId { get; set; }

        // Navigation
        public ProductImage? ProductImage { get; set; }
        public AttributeValue? AttributeValue { get; set; }
    }
}

