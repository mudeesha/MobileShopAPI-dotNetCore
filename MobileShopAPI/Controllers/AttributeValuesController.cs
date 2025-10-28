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
        
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk([FromBody] AttributeValueCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.CreateBulkAsync(dto);
                return Ok(new { message = "Attribute values created successfully.", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] AttributeValueUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _service.UpdateAsync(dto);
                return Ok(new { message = "Attribute values updated successfully.", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
