namespace MobileShopAPI.DTOs;

public class ImageAssignmentDto
{
    public int ProductId { get; set; }
    public int ProductImageId { get; set; }
    public bool IsDefault { get; set; }
}