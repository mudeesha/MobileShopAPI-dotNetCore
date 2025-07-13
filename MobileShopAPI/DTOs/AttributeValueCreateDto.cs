namespace MobileShopAPI.DTOs
{
    public class AttributeValueCreateDto
    {
        public int AttributeTypeId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
