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
        
        public async Task UpdateAsync(int id, AttributeTypeUpdateDto dto)
        {
            var existingAttributeType = await _repo.GetByIdAsync(id);
            if (existingAttributeType == null)
                throw new KeyNotFoundException($"Attribute type with ID {id} not found.");

            existingAttributeType.Name = dto.Name;

            await _repo.UpdateAsync(existingAttributeType);
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var existingType = await _repo.GetByIdAsync(id);
            if (existingType == null)
                return false;

            await _repo.DeleteAsync(existingType);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
