namespace MobileShopAPI.Models
{
    public class AttributeValue
    {
        public int Id { get; set; }

        public int AttributeTypeId { get; set; }
        
        public AttributeType AttributeType { get; set; } = null!;

        public string Value { get; set; } = string.Empty;

        public ICollection<ProductAttribute> ProductAttributes { get; set; } = new List<ProductAttribute>();
    }
}

