using Microsoft.AspNetCore.Mvc;
using MobileShopAPI.DTOs;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttributeValuesController : ControllerBase
    {
        private readonly IAttributeValueService _service;

        public AttributeValuesController(IAttributeValueService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<AttributeValueDto>>> GetAll()
        {
            var values = await _service.GetAllAsync();
            return Ok(values);
        }

        [HttpPost]
        public async Task<ActionResult<AttributeValueDto>> Create(AttributeValueCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }
    }
}
