using MobileShopAPI.DTOs;

namespace MobileShopAPI.Services.Interfaces
{
    public interface IAttributeValueService
    {
        Task<List<object>> GetAllAsync(); // return nested structure
        Task<AttributeValueDto> CreateAsync(AttributeValueCreateDto dto);
        Task<(AttributeTypeDto Type, List<AttributeValueDto> Values)> CreateTypeWithValuesAsync(AttributeTypeWithValuesCreateDto dto);
    }
}
