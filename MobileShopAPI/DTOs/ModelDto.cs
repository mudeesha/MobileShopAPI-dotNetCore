namespace MobileShopAPI.DTOs
{
    public class ModelDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Optional: Include Brand ID to relate
        public int BrandId { get; set; }
    }
}
