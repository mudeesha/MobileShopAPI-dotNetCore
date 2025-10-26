using MobileShopAPI.DTOs;
using MobileShopAPI.Models;
using System.Threading.Tasks;

namespace MobileShopAPI.Services.Interfaces
{
    public interface IProductImageService
    {
        Task<ProductImage> AddProductImageAsync(ProductImageCreateDto dto);
    }
}
