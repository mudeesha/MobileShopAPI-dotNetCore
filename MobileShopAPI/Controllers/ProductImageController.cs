using Microsoft.AspNetCore.Mvc;
using MobileShopAPI.DTOs;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Controllers
{
    /// <summary>
    /// Controller for managing product images and their assignments to products
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _imageService;

        public ProductImageController(IProductImageService imageService)
        {
            _imageService = imageService;
        }

        /// <summary>
        /// Get all product images with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 20, max: 100)</param>
        /// <returns>Paginated list of product images</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllImages(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;
            
            var images = await _imageService.GetAllImagesAsync(pageNumber, pageSize);
            var totalCount = await _imageService.GetTotalImageCountAsync();
            
            return Ok(new
            {
                Data = images,
                Pagination = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }

        /// <summary>
        /// Get a specific product image by ID
        /// </summary>
        /// <param name="id">Product image ID</param>
        /// <returns>Product image details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImageById(int id)
        {
            var image = await _imageService.GetImageByIdAsync(id);
            if (image == null) return NotFound(new { message = "Image not found" });
            
            return Ok(image);
        }

        /// <summary>
        /// Get all images assigned to a specific product
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <returns>List of images for the product</returns>
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetImagesByProduct(int productId)
        {
            var images = await _imageService.GetImagesByProductAsync(productId);
            return Ok(images);
        }

        /// <summary>
        /// Get all products that use a specific image
        /// </summary>
        /// <param name="imageId">Product image ID</param>
        /// <returns>List of products using the image</returns>
        [HttpGet("{imageId}/products")]
        public async Task<IActionResult> GetProductsByImage(int imageId)
        {
            var products = await _imageService.GetProductsByImageAsync(imageId);
            return Ok(products);
        }

        /// <summary>
        /// Create a new product image and optionally assign it to products
        /// </summary>
        /// <param name="dto">Product image creation data</param>
        /// <returns>Created product image</returns>
        [HttpPost]
        public async Task<IActionResult> AddImage([FromBody] ProductImageCreateDto dto)
        {
            var image = await _imageService.CreateImageAsync(dto);
            
            // Assign to selected products if provided
            if (dto.ProductIds != null && dto.ProductIds.Any())
            {
                foreach (var productId in dto.ProductIds)
                {
                    await _imageService.AssignImageToProductAsync(new ImageAssignmentDto
                    {
                        ProductId = productId,
                        ProductImageId = image.Id,
                        IsDefault = dto.IsDefault
                    });
                }
            }

            return Ok(image);
        }

        /// <summary>
        /// Update an existing product image and its assignments
        /// </summary>
        /// <param name="id">Product image ID</param>
        /// <param name="dto">Product image update data</param>
        /// <returns>Success message</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImage(int id, [FromBody] ProductImageUpdateDto dto)
        {
            // Update image details
            var updateResult = await _imageService.UpdateImageAsync(id, dto);
            if (!updateResult) return NotFound(new { message = "Image not found" });

            // Remove existing assignments
            var existingProducts = await _imageService.GetProductsByImageAsync(id);
            foreach (var product in existingProducts)
            {
                await _imageService.RemoveImageFromProductAsync(product.Id, id);
            }

            // Add new assignments if provided
            if (dto.ProductIds != null && dto.ProductIds.Any())
            {
                foreach (var productId in dto.ProductIds)
                {
                    await _imageService.AssignImageToProductAsync(new ImageAssignmentDto
                    {
                        ProductId = productId,
                        ProductImageId = id,
                        IsDefault = dto.IsDefault
                    });
                }
            }

            return Ok(new { message = "Image updated successfully" });
        }

        /// <summary>
        /// Delete a product image
        /// </summary>
        /// <param name="id">Product image ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var result = await _imageService.DeleteImageAsync(id);
            if (!result) return NotFound(new { message = "Image not found" });
            
            return Ok(new { message = "Image deleted successfully" });
        }

        /// <summary>
        /// Assign an existing image to a product
        /// </summary>
        /// <param name="dto">Image assignment data</param>
        /// <returns>Success message</returns>
        [HttpPost("assign")]
        public async Task<IActionResult> AssignImageToProduct([FromBody] ImageAssignmentDto dto)
        {
            var result = await _imageService.AssignImageToProductAsync(dto);
            if (!result) return NotFound(new { message = "Image not found" });
            
            return Ok(new { message = "Image assigned to product successfully" });
        }

        /// <summary>
        /// Remove an image assignment from a product
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="productImageId">Product image ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("unassign/{productId}/{productImageId}")]
        public async Task<IActionResult> RemoveImageFromProduct(int productId, int productImageId)
        {
            await _imageService.RemoveImageFromProductAsync(productId, productImageId);
            return Ok(new { message = "Image removed from product successfully" });
        }

        /// <summary>
        /// Set a specific image as default for a product
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="productImageId">Product image ID to set as default</param>
        /// <returns>Success message</returns>
        [HttpPut("set-default/{productId}/{productImageId}")]
        public async Task<IActionResult> SetDefaultImage(int productId, int productImageId)
        {
            await _imageService.SetDefaultImageAsync(productId, productImageId);
            return Ok(new { message = "Default image set successfully" });
        }
    }
}