using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileShopAPI.Models
{
    public class ProductInventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public int StockQuantity { get; set; }

        public decimal Price { get; set; }

        // Navigation
        public Product Product { get; set; } = null!;

        public ICollection<InventoryAttributeValue> InventoryAttributeValues { get; set; } = new List<InventoryAttributeValue>();


    }
}
