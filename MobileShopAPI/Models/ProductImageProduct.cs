using System.ComponentModel.DataAnnotations;

namespace MobileShopAPI.Models
{
    public class ProductImageProduct
    {
        public int ProductImageId { get; set; }
        public ProductImage ProductImage { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}
