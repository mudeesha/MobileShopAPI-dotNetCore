using Microsoft.AspNetCore.Mvc;
using MobileShopAPI.DTOs;
using MobileShopAPI.Services.Interfaces;
using MobileShopAPI.DTOs;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelsController : ControllerBase
    {
        private readonly IModelService _modelService;

        public ModelsController(IModelService modelService)
        {
            _modelService = modelService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ModelDto>>> GetAll() =>
            Ok(await _modelService.GetAllModelsAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<ModelDto>> GetById(int id)
        {
            var model = await _modelService.GetModelByIdAsync(id);
            return model is null ? NotFound() : Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult<ModelDto>> Create(ModelDto dto)
        {
            var created = await _modelService.CreateModelAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _modelService.DeleteModelAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
