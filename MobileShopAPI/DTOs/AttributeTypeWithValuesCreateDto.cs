 namespace MobileShopAPI.DTOs
{
    public class AttributeTypeWithValuesCreateDto
    {
        public string Type { get; set; } = string.Empty;
        public List<string> Values { get; set; } = new();
    }
}
