using System.ComponentModel.DataAnnotations;

namespace MobileShopAPI.DTOs
{
    public class ProductImageUpdateDto
    {
        [Required]
        public string ImageUrl { get; set; } = null!;
        
        [Required]
        [MinLength(1, ErrorMessage = "At least one product must be selected")]
        public List<int> ProductIds { get; set; } = new();

        public bool IsDefault { get; set; }
    }
}