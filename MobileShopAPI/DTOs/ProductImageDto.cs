namespace MobileShopAPI.DTOs
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsDefault { get; set; } 
    }
}