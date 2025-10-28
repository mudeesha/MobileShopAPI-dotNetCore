using Microsoft.AspNetCore.Mvc;
using MobileShopAPI.DTOs;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _imageService;

        public ProductImageController(IProductImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost]
        public async Task<IActionResult> AddImage([FromBody] ProductImageCreateDto dto)
        {
            var image = await _imageService.CreateImageAsync(dto);
            
            // Assign to selected products
            foreach (var productId in dto.ProductIds)
            {
                await _imageService.AssignImageToProductAsync(new ImageAssignmentDto
                {
                    ProductId = productId,
                    ProductImageId = image.Id,
                    IsDefault = dto.IsDefault
                });
            }

            return Ok(image);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImage(int id, [FromBody] ProductImageUpdateDto dto)
        {
            // Update image details (description)
            await _imageService.UpdateImageAsync(id, dto);

            // Remove existing assignments
            var existingProducts = await _imageService.GetProductsByImageAsync(id);
            foreach (var product in existingProducts)
            {
                await _imageService.RemoveImageFromProductAsync(product.Id, id);
            }

            // Add new assignments
            foreach (var productId in dto.ProductIds)
            {
                await _imageService.AssignImageToProductAsync(new ImageAssignmentDto
                {
                    ProductId = productId,
                    ProductImageId = id,
                    IsDefault = dto.IsDefault
                });
            }

            return Ok(new { message = "Image updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var result = await _imageService.DeleteImageAsync(id);
            if (!result) return BadRequest("Image not found");
            return Ok(new { message = "Image deleted successfully" });
        }
        
        //will ipplement get all api
    }
}