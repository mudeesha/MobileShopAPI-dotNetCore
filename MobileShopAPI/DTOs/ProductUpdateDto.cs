using System.ComponentModel.DataAnnotations;

namespace MobileShopAPI.DTOs
{
    public class ProductUpdateDto
    {
        [Required]
        public int ModelId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "StockQuantity cannot be negative.")]
        public int StockQuantity { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one attribute value must be selected.")]
        public List<int> AttributeValueIds { get; set; } = new();
    }
}