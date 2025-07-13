using MobileShopAPI.DTOs;

namespace MobileShopAPI.Services.Interfaces
{
    public interface IAttributeValueService
    {
        Task<List<AttributeValueDto>> GetAllAsync();
        Task<AttributeValueDto> CreateAsync(AttributeValueCreateDto dto);
    }
}
