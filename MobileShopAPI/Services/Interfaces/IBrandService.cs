using MobileShopAPI.DTOs;

namespace MobileShopAPI.Services.Interfaces
{
    public interface IBrandService
    {
        Task<List<BrandDto>> GetAllBrandsAsync();
        Task<BrandDto?> GetBrandByIdAsync(int id);
        Task<BrandDto> CreateBrandAsync(BrandDto brandDto);
        Task<bool> DeleteBrandAsync(int id);
    }
}
