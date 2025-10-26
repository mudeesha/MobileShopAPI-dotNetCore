using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MobileShopAPI.Validation;

namespace MobileShopAPI.DTOs
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Model ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Model ID must be a positive number")]
        public int ModelId { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "At least one attribute value is required")]
        [MinLength(1, ErrorMessage = "At least one attribute value is required")]
        [NoDuplicateIds(ErrorMessage = "Duplicate attribute values are not allowed")]
        [ExistingAttributeValues(ErrorMessage = "One or more attribute values do not exist")]
        public List<int> AttributeValueIds { get; set; } = new();
    }
}