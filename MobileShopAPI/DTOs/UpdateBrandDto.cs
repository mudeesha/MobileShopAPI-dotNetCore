using System.ComponentModel.DataAnnotations;

namespace MobileShopAPI.DTOs
{
    public class UpdateBrandDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;
    }
}