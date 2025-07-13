namespace MobileShopAPI.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;

        public int ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;

        public List<AttributeValueDto> Attributes { get; set; } = new();
    }
}
