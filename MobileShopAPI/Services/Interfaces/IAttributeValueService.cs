using MobileShopAPI.DTOs;

namespace MobileShopAPI.Services.Interfaces
{
    public interface IAttributeValueService
    {
        Task<List<object>> GetAllAsync();
        Task<List<AttributeValueDto>> CreateBulkAsync(AttributeValueCreateDto dto);
        Task<List<AttributeValueDto>> UpdateAsync(AttributeValueUpdateDto dto);

        Task<(AttributeTypeDto Type, List<AttributeValueDto> Values)> CreateTypeWithValuesAsync(AttributeTypeWithValuesCreateDto dto);
    }
}
