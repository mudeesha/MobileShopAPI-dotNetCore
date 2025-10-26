public class ProductImageCreateDto
{
    public int ProductId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public bool IsDefault { get; set; }
    public List<int>? AppearanceAttributeValueIds { get; set; }
}