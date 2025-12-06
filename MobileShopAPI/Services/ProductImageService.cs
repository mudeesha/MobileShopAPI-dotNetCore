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

        // ✅ Add these missing methods for GET endpoints:

        public async Task<List<ProductImageDto>> GetAllImagesAsync(int pageNumber = 1, int pageSize = 20)
        {
            var images = await _imageRepo.GetAllAsync(pageNumber, pageSize);
            
            return images.Select(img => new ProductImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl,
                // Add other properties if needed
            }).ToList();
        }

        public async Task<ProductImageDto?> GetImageByIdAsync(int id)
        {
            var image = await _imageRepo.GetByIdAsync(id);
            if (image == null) return null;

            return new ProductImageDto
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl,
                // Add other properties if needed
            };
        }

        // ✅ GET: Get total count for pagination
        public async Task<int> GetTotalImageCountAsync()
        {
            return await _imageRepo.GetTotalCountAsync();
        }

        // Your existing methods...
        public async Task<ProductImageDto> CreateImageAsync(ProductImageCreateDto dto)
        {
            var productImage = new ProductImage
            {
                ImageUrl = dto.ImageUrl,
            };

            await _imageRepo.AddAsync(productImage);
            await _imageRepo.SaveChangesAsync();

            return new ProductImageDto
            {
                Id = productImage.Id,
                ImageUrl = productImage.ImageUrl,
            };
        }

        public async Task<bool> UpdateImageAsync(int id, ProductImageUpdateDto dto)
        {
            var image = await _imageRepo.GetByIdAsync(id);
            if (image == null) return false;

            image.ImageUrl = dto.ImageUrl;

            await _imageRepo.UpdateAsync(image);
            await _imageRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignImageToProductAsync(ImageAssignmentDto dto)
        {
            var image = await _imageRepo.GetByIdAsync(dto.ProductImageId);
            if (image == null) return false;

            await _imageRepo.AssignImageToProductAsync(
                dto.ProductId, 
                dto.ProductImageId, 
                dto.IsDefault);
            
            await _imageRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveImageFromProductAsync(int productId, int productImageId)
        {
            await _imageRepo.RemoveImageFromProductAsync(productId, productImageId);
            await _imageRepo.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductImageDto>> GetImagesByProductAsync(int productId)
        {
            var images = await _imageRepo.GetImagesByProductAsync(productId);
            
            return images.Select(img => new ProductImageDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl,
            }).ToList();
        }

        public async Task<List<ProductDto>> GetProductsByImageAsync(int productImageId)
        {
            var products = await _imageRepo.GetProductsByImageAsync(productImageId);
            
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                SKU = p.SKU,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ModelId = p.ModelId,
                BrandId = p.Model?.BrandId ?? 0,
                BrandName = p.Model?.Brand?.Name ?? "",
                ModelName = p.Model?.Name ?? ""
            }).ToList();
        }

        public async Task<bool> SetDefaultImageAsync(int productId, int productImageId)
        {
            await _imageRepo.SetDefaultImageAsync(productId, productImageId);
            await _imageRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteImageAsync(int id)
        {
            var image = await _imageRepo.GetByIdAsync(id);
            if (image == null) return false;

            await _imageRepo.DeleteAsync(image);
            await _imageRepo.SaveChangesAsync();
            return true;
        }
    }
}