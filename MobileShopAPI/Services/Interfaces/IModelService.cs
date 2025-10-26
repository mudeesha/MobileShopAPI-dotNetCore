using MobileShopAPI.DTOs;

namespace MobileShopAPI.Services.Interfaces
{
    public interface IModelService
    {
        Task<List<ModelDto>> GetAllModelsAsync();           // Returns ModelDto
        Task<ModelDto?> GetModelByIdAsync(int id);          // Returns ModelDto  
        Task<ModelDto> CreateModelAsync(ModelCreateDto modelDto); // Takes ModelCreateDto, returns ModelDto
        Task<bool> DeleteModelAsync(int id);
    }
}