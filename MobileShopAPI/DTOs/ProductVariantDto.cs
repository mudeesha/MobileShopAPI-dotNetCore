namespace MobileShopAPI.DTOs
{
    public class ProductVariantDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public List<AttributeValueDto> Attributes { get; set; } = new();
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public string? DefaultImageUrl { get; set; }
        public List<string> Images { get; set; } = new();
    }
}