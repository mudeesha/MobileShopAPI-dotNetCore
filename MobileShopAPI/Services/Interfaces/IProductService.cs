using MobileShopAPI.DTOs;

namespace MobileShopAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductDto>> GetAllAsync(string? searchTerm = null, int pageNumber = 1, int pageSize = 10);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(ProductCreateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<ProductDto> UpdateAsync(int id, ProductUpdateDto dto);
    }
}