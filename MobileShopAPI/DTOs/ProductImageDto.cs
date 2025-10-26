namespace MobileShopAPI.DTOs
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsDefault { get; set; }
        public List<int> AttributeValueIds { get; set; } = new();
    }
}
