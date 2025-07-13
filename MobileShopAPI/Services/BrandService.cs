using MobileShopAPI.DTOs;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;

        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<List<BrandDto>> GetAllBrandsAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            return brands.Select(b => new BrandDto { Id = b.Id, Name = b.Name }).ToList();
        }

        public async Task<BrandDto?> GetBrandByIdAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            return brand is null ? null : new BrandDto { Id = brand.Id, Name = brand.Name };
        }

        public async Task<BrandDto> CreateBrandAsync(BrandDto brandDto)
        {
            var brand = new Brand { Name = brandDto.Name };
            await _brandRepository.AddAsync(brand);
            await _brandRepository.SaveChangesAsync();

            return new BrandDto { Id = brand.Id, Name = brand.Name };
        }

        public async Task<bool> DeleteBrandAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand is null)
                return false;

            await _brandRepository.DeleteAsync(brand);
            await _brandRepository.SaveChangesAsync();
            return true;
        }
    }
}
