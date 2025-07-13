namespace MobileShopAPI.DTOs
{
    public class AttributeValueDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // e.g., RAM
        public string Value { get; set; } = string.Empty; // e.g., 4GB
    }
}
