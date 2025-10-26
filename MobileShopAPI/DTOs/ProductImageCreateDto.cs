namespace MobileShopAPI.DTOs
{
    public class ProductImageCreateDto
    {
        public string ImageUrl { get; set; } = null!;
        public string? Description { get; set; }
    }
}