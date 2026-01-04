// Controllers/CustomerModelsController.cs
using Microsoft.AspNetCore.Mvc;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerModelsController : ControllerBase
    {
        private readonly ICustomerModelService _customerModelService;

        public CustomerModelsController(ICustomerModelService customerModelService)
        {
            _customerModelService = customerModelService;
        }

        [HttpGet("listing")]
        public async Task<IActionResult> GetModelListing()
        {
            var models = await _customerModelService.GetModelListingAsync();
            return Ok(models);
        }

        [HttpGet("by-attribute-values")]
        public async Task<IActionResult> GetProductByAttributeValueIds(
            [FromQuery] int modelId,
            [FromQuery] string attributeValueIds)
        {
            if (modelId <= 0)
                return BadRequest("Model ID must be greater than 0");

            if (string.IsNullOrWhiteSpace(attributeValueIds))
                return BadRequest("attributeValueIds parameter is required");

            // Parse IDs
            var ids = attributeValueIds.Split(',')
                .Where(idStr => int.TryParse(idStr.Trim(), out int id) && id > 0)
                .Select(idStr => int.Parse(idStr.Trim()))
                .ToList();

            if (!ids.Any())
                return BadRequest("Provide valid attribute value IDs (e.g., 12,14)");

            var product = await _customerModelService.GetProductByAttributeValueIdsAsync(modelId, ids);

            if (product == null)
                return NotFound($"No product found with the specified attributes");

            return Ok(product);
        }
    }
}