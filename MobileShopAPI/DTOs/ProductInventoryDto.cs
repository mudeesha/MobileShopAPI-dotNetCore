namespace MobileShopAPI.DTOs
{
    public class ProductInventoryDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int StockQuantity { get; set; }

        public decimal Price { get; set; }
        public List<int> AttributeValueIds { get; set; } = new();
    }
}

