using System.ComponentModel.DataAnnotations;

namespace MobileShopAPI.DTOs
{
    public class AttributeValueCreateDto
    {
        [Required]
        public int AttributeTypeId { get; set; }

        [Required]
        [MinLength(1)]
        public List<string> Values { get; set; } = new List<string>();
    }
}
