using System.ComponentModel.DataAnnotations;

namespace MobileShopAPI.DTOs;

public class AttributeValueUpdateDto
{
    [Required]
    public int AttributeTypeId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one value must be provided.")]
    public List<string> Values { get; set; } = new List<string>();
}