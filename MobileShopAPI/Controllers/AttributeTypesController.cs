using Microsoft.AspNetCore.Mvc;
using MobileShopAPI.DTOs;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttributeTypesController : ControllerBase
    {
        private readonly IAttributeTypeService _service;

        public AttributeTypesController(IAttributeTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<AttributeTypeDto>>> GetAll()
        {
            var types = await _service.GetAllAsync();
            return Ok(types);
        }

        [HttpPost]
        public async Task<ActionResult<AttributeTypeDto>> Create(AttributeTypeCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AttributeTypeUpdateDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok(new { message = "Attribute type updated successfully" });
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = $"Attribute type with ID {id} not found." });

            return Ok(new { message = "Attribute type deleted successfully." });
        }
        
        //willimplement get one api
    }
}
