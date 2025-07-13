using MobileShopAPI.DTOs;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Services
{
    public class AttributeValueService : IAttributeValueService
    {
        private readonly IAttributeValueRepository _repo;

        public AttributeValueService(IAttributeValueRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<AttributeValueDto>> GetAllAsync()
        {
            var values = await _repo.GetAllAsync();
            return values.Select(av => new AttributeValueDto
            {
                Id = av.Id,
                Type = av.AttributeType.Name,
                Value = av.Value
            }).ToList();
        }

        public async Task<AttributeValueDto> CreateAsync(AttributeValueCreateDto dto)
        {
            var av = new AttributeValue
            {
                AttributeTypeId = dto.AttributeTypeId,
                Value = dto.Value
            };

            await _repo.AddAsync(av);
            await _repo.SaveChangesAsync();

            return new AttributeValueDto
            {
                Id = av.Id,
                Type = "", // Optional — you can load type name if needed
                Value = av.Value
            };
        }
    }
}
