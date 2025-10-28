using System.ComponentModel.DataAnnotations;
namespace MobileShopAPI.DTOs;

public class AttributeTypeUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}