using MobileShopAPI.DTOs;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _imageRepo;

        public ProductImageService(IProductImageRepository imageRepo)
        {
            _imageRepo = imageRepo;
        }

        public async Task<ProductImage> AddProductImageAsync(ProductImageCreateDto dto) 
        {
            var image = new ProductImage
            {
                ImageUrl = dto.ImageUrl,
                IsDefault = dto.IsDefault,
                ProductId = dto.ProductId
            };

            await _imageRepo.AddAsync(image);
            await _imageRepo.SaveChangesAsync();

            return image;
        }



    }
}
