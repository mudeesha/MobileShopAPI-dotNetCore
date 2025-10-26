namespace MobileShopAPI.DTOs
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<int> ProductIds { get; set; } = new(); // Products using this image
    }
}