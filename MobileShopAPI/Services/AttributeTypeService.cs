using MobileShopAPI.DTOs;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Services
{
    public class AttributeTypeService : IAttributeTypeService
    {
        private readonly IAttributeTypeRepository _repo;

        public AttributeTypeService(IAttributeTypeRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<AttributeTypeDto>> GetAllAsync()
        {
            var types = await _repo.GetAllAsync();
            return types.Select(t => new AttributeTypeDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();
        }

        public async Task<AttributeTypeDto> CreateAsync(AttributeTypeCreateDto dto)
        {
            var existingType = await _repo.GetByNameAsync(dto.Name);
            if (existingType != null)
            {
                throw new Exception($"Attribute type '{dto.Name}' already exists.");
            }

            var type = new AttributeType
            {
                Name = dto.Name
            };

            await _repo.AddAsync(type);
            await _repo.SaveChangesAsync();

            return new AttributeTypeDto
            {
                Id = type.Id,
                Name = type.Name
            };
        }
    }
}
