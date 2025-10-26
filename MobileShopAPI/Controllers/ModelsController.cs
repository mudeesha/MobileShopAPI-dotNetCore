using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopAPI.DTOs;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ModelsController : ControllerBase
    {
        private readonly IModelService _modelService;

        public ModelsController(IModelService modelService)
        {
            _modelService = modelService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Customer, Staff")]
        public async Task<ActionResult<List<ModelDto>>> GetAll() =>
            Ok(await _modelService.GetAllModelsAsync());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Customer, Staff")]
        public async Task<ActionResult<ModelDto>> GetById(int id)
        {
            var model = await _modelService.GetModelByIdAsync(id);
            return model is null ? NotFound() : Ok(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ModelDto>> Create(ModelDto dto)
        {
            var created = await _modelService.CreateModelAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin, Customer, Staff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _modelService.DeleteModelAsync(id);
            return result ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Admin, Customer, Staff")]
        [HttpGet("with-products")]
        public async Task<IActionResult> GetAllModelsWithProducts()
        {
            var result = await _modelService.GetAllModelsWithProductsAsync();
            return Ok(result);
        }
    }
}
