using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopAPI.DTOs;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductInventoryController : ControllerBase
    {
        private readonly IProductInventoryService _inventoryService;

        public ProductInventoryController(IProductInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var result = await _inventoryService.GetByProductIdAsync(productId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductInventoryCreateDto dto)
        {
            var created = await _inventoryService.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPost("match")]
        public async Task<IActionResult> GetExactMatch([FromBody] ProductInventoryMatchRequestDto request)
        {
            var match = await _inventoryService.GetExactMatchAsync(request.ProductId, request.AttributeValueIds);
            return match == null ? NotFound("No exact inventory match") : Ok(match);
        }
    }

    public class ProductInventoryMatchRequestDto
    {
        public int ProductId { get; set; }
        public List<int> AttributeValueIds { get; set; } = new();
    }
}
