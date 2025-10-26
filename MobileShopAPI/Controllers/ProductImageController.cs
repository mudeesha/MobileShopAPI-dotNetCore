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

        [HttpPost("add-image")]
        public async Task<IActionResult> AddImage([FromBody] ProductImageCreateDto dto)
        {
            if (dto.ProductId <= 0) // Fixed: ProductId instead of ProductIds
                return BadRequest("A valid product ID is required.");

            var image = await _imageService.AddProductImageAsync(dto);
            return Ok(image);
        }
    }
}
