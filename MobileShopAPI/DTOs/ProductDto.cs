namespace MobileShopAPI.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = null!;
        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!;
        public int ModelId { get; set; }
        public string ModelName { get; set; } = null!;
        public List<AttributeValueDto> Attributes { get; set; } = new();
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
        public string? DefaultImageUrl { get; set; }
    }
}
