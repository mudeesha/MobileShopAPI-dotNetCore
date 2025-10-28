namespace MobileShopAPI.Models
{
    public class AttributeType
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public ICollection<AttributeValue> AttributeValues { get; set; } = new List<AttributeValue>();
    }
}
