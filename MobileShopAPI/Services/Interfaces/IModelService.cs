using MobileShopAPI.DTOs;

namespace MobileShopAPI.Services.Interfaces
{
    public interface IModelService
    {
        Task<List<ModelDto>> GetAllModelsAsync();
        Task<ModelDto?> GetModelByIdAsync(int id);
        Task<ModelDto> CreateModelAsync(ModelCreateDto modelDto);
        Task<ModelDto> UpdateAsync(int id, UpdateModelDto updateModelDto);
        Task<bool> DeleteModelAsync(int id);
    }
}