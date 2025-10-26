namespace MobileShopAPI.DTOs
{
    public class ModelWithProductsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public List<ProductDto> Products { get; set; } = new();
    }
}
