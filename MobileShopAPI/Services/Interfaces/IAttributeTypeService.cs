using MobileShopAPI.DTOs;

namespace MobileShopAPI.Services.Interfaces
{
    public interface IAttributeTypeService
    {
        Task<List<AttributeTypeDto>> GetAllAsync();
        Task<AttributeTypeDto> CreateAsync(AttributeTypeCreateDto dto);
        Task UpdateAsync(int id, AttributeTypeUpdateDto dto);
        Task<bool> DeleteAsync(int id);


    }
}
