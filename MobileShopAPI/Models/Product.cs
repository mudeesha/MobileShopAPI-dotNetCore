using Microsoft.AspNetCore.Mvc;
using MobileShopAPI.Models;

namespace MobileShopAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        public int ModelId { get; set; }
        public Model? Model { get; set; }

        public string SKU { get; set; } = string.Empty;

        public ICollection<ProductAttribute> ProductAttributes { get; set; } = new List<ProductAttribute>();
        public List<ProductInventory> ProductInventories { get; set; } = new();

        public List<ProductImage> ProductImages { get; set; } = new();

    }

}
