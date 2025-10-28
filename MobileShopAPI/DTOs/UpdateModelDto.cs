using System.ComponentModel.DataAnnotations;

namespace MobileShopAPI.DTOs
{
    public class UpdateModelDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "BrandId is required.")]
        public int BrandId { get; set; }
    }
}