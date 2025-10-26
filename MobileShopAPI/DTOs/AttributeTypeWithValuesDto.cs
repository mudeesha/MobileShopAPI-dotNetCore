namespace MobileShopAPI.DTOs
{
    public class AttributeTypeWithValuesDto
    {
        public int AttributeTypeId { get; set; }
        public string AttributeTypeName { get; set; }
        public List<AttributeValueItemDto> Values { get; set; }
    }
}
