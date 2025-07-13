namespace MobileShopAPI.DTOs
{
    public class ProductCreateDto
    {
        public int BrandId { get; set; }
        public int ModelId { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public List<int> AttributeValueIds { get; set; } = new(); // RAM, Color, etc.
    }
}
