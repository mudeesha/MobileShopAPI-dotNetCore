using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopAPI.DTOs;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class AttributeValuesController : ControllerBase
    {
        private readonly IAttributeValueService _service;

        public AttributeValuesController(IAttributeValueService service)
        {
            _service = service;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin, Customer, Staff")]
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

        [HttpPost("type-with-values")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTypeWithValues(AttributeTypeWithValuesCreateDto dto)
        {
            try
            {
                var (typeDto, valueDtos) = await _service.CreateTypeWithValuesAsync(dto);
                return Ok(new
                {
                    Message = "Attribute type and values created/updated successfully.",
                    Type = typeDto,
                    Values = valueDtos
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


    }
}
