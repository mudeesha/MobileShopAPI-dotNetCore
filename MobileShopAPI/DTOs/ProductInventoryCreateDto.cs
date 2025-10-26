namespace MobileShopAPI.DTOs
{
    public class ProductInventoryCreateDto
    {
        public int ProductId { get; set; }
        public int StockQuantity { get; set; }
        public List<int> AttributeValueIds { get; set; } = new();
        public decimal Price { get; set; }
    }
}
