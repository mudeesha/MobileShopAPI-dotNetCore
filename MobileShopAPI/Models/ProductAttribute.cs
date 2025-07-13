using System.ComponentModel.DataAnnotations.Schema;

namespace MobileShopAPI.Models
{
    public class ProductAttribute
    {
        public int Id { get; set; }

        public int AttributeValueId { get; set; }
        [ForeignKey(nameof(AttributeValueId))]
        public AttributeValue AttributeValue { get; set; } = null!;

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
    }
}

