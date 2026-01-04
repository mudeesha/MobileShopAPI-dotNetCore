namespace MobileShopAPI.DTOs
{
    public class ModelListingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? DefaultImageUrl { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int TotalStock { get; set; }
        public bool HasStock => TotalStock > 0;
        public Dictionary<string, List<string>> AttributeOptions { get; set; } = new();
        public List<ProductVariantDto> Products { get; set; } = new();
    }
}