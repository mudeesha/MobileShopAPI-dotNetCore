using MobileShopAPI.DTOs;

namespace MobileShopAPI.Services.Interfaces
{
    public interface IModelService
    {
        Task<List<ModelDto>> GetAllModelsAsync();
        Task<ModelDto?> GetModelByIdAsync(int id);
        Task<ModelDto> CreateModelAsync(ModelDto modelDto);
        Task<bool> DeleteModelAsync(int id);
        Task<List<ModelWithProductsDto>> GetAllModelsWithProductsAsync();
    }
}
