using MobileShopAPI.DTOs;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Services
{
    public class AttributeValueService : IAttributeValueService
    {
        private readonly IAttributeValueRepository _repo;
        private readonly IAttributeTypeRepository _typeRepo;

        public AttributeValueService(IAttributeValueRepository repo, IAttributeTypeRepository typeRepo)
        {
            _repo = repo;
            _typeRepo = typeRepo;
        }

        public async Task<List<object>> GetAllAsync()
        {
            var attributeTypes = await _typeRepo.GetAllAsync();
            var attributeValues = await _repo.GetAllAsync();

            var result = attributeTypes.Select(type => new
            {
                attributeTypeId = type.Id,
                attributeTypeName = type.Name,
                attributeValues = attributeValues
                    .Where(v => v.AttributeTypeId == type.Id)
                    .Select(v => new
                    {
                        id = v.Id,
                        value = v.Value
                    })
                    .ToList()
            }).ToList<object>();

            return result;
        }

        public async Task<List<AttributeValueDto>> CreateBulkAsync(AttributeValueCreateDto dto)
        {
            // 1️⃣ Check if attribute type exists
            var type = await _typeRepo.GetByIdAsync(dto.AttributeTypeId);
            if (type == null)
                throw new Exception($"Attribute type with ID {dto.AttributeTypeId} not found.");

            // 2️⃣ Remove duplicate values in request itself
            var distinctValues = dto.Values.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (!distinctValues.Any())
                throw new Exception("No valid attribute values provided.");

            var createdValues = new List<AttributeValueDto>();

            foreach (var value in distinctValues)
            {
                // 3️⃣ Check for existing value in DB
                var existingValue = await _repo.GetByTypeAndValueAsync(dto.AttributeTypeId, value);
                if (existingValue != null)
                    continue; // Skip duplicate

                var av = new AttributeValue
                {
                    AttributeTypeId = dto.AttributeTypeId,
                    Value = value
                };

                await _repo.AddAsync(av);

                createdValues.Add(new AttributeValueDto
                {
                    Id = av.Id,
                    Type = type.Name,
                    Value = av.Value
                });
            }

            if (!createdValues.Any())
                throw new Exception("All provided values already exist for this attribute type.");

            await _repo.SaveChangesAsync();

            return createdValues;
        }
        
        public async Task<List<AttributeValueDto>> UpdateAsync(AttributeValueUpdateDto dto)
        {
            // 1️⃣ Validate attribute type exists
            var type = await _typeRepo.GetByIdAsync(dto.AttributeTypeId);
            if (type == null)
                throw new Exception($"Attribute type with ID {dto.AttributeTypeId} not found.");

            // 2️⃣ Remove duplicates in request
            var distinctValues = dto.Values.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (!distinctValues.Any())
                throw new Exception("No valid attribute values provided.");

            // 3️⃣ Load existing values from DB
            var existingValues = (await _repo.GetByAttributeTypeIdAsync(dto.AttributeTypeId)).ToList();
            var existingValueStrings = existingValues.Select(ev => ev.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

            // 4️⃣ Determine values to delete
            var toDelete = existingValues.Where(ev => !distinctValues.Contains(ev.Value, StringComparer.OrdinalIgnoreCase)).ToList();
            foreach (var ev in toDelete)
            {
                await _repo.DeleteAsync(ev);
            }

            // 5️⃣ Determine values to add
            var toAdd = distinctValues.Where(v => !existingValueStrings.Contains(v, StringComparer.OrdinalIgnoreCase)).ToList();
            var addedValues = new List<AttributeValueDto>();

            foreach (var value in toAdd)
            {
                var av = new AttributeValue
                {
                    AttributeTypeId = dto.AttributeTypeId,
                    Value = value
                };
                await _repo.AddAsync(av);

                addedValues.Add(new AttributeValueDto
                {
                    Id = av.Id,
                    Type = type.Name,
                    Value = av.Value
                });
            }

            // 6️⃣ Save changes
            await _repo.SaveChangesAsync();

            // 7️⃣ Return **current full state of attribute values**
            var finalValues = (await _repo.GetByAttributeTypeIdAsync(dto.AttributeTypeId))
                .Select(av => new AttributeValueDto
                {
                    Id = av.Id,
                    Type = type.Name,
                    Value = av.Value
                }).ToList();

            return finalValues;
        }

        //will implement delete api
        
        //well implement get one api

        public async Task<(AttributeTypeDto Type, List<AttributeValueDto> Values)> CreateTypeWithValuesAsync(AttributeTypeWithValuesCreateDto dto)
        {
            // 1. Check if attribute type exists by name (case-insensitive)
            var existingType = await _typeRepo.GetByNameAsync(dto.Type);

            if (existingType == null)
            {
                // 2. Type doesn't exist — create new type and all values
                var newType = new AttributeType { Name = dto.Type };
                await _typeRepo.AddAsync(newType);
                await _typeRepo.SaveChangesAsync();

                // Remove duplicates from input values
                var distinctValues = dto.Values.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                var newValues = distinctValues.Select(v => new AttributeValue
                {
                    AttributeTypeId = newType.Id,
                    Value = v
                }).ToList();

                await _repo.AddRangeAsync(newValues);
                await _repo.SaveChangesAsync();

                var typeDto = new AttributeTypeDto { Id = newType.Id, Name = newType.Name };
                var valueDtos = newValues.Select(v => new AttributeValueDto
                {
                    Id = v.Id,
                    Type = newType.Name,
                    Value = v.Value
                }).ToList();

                return (typeDto, valueDtos);
            }
            else
            {
                // 3. Type exists — get existing values for this type
                var existingValues = await _repo.GetByAttributeTypeIdAsync(existingType.Id);
                var existingValueSet = existingValues.Select(v => v.Value.ToLower()).ToHashSet();

                // 4. Filter input values: only keep those not already existing
                var distinctInputValues = dto.Values.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var newValuesToAdd = distinctInputValues
                    .Where(v => !existingValueSet.Contains(v.ToLower()))
                    .ToList();

                if (newValuesToAdd.Count == 0)
                {
                    // All values already exist — nothing to add, optionally throw or just return existing
                    // throw new Exception($"Attribute type '{existingType.Name}' already has all these values.");
                    // Or just return existing values:
                    //var typeDto = new AttributeTypeDto { Id = existingType.Id, Name = existingType.Name };
                    //var valueDtos = existingValues.Select(v => new AttributeValueDto
                    //{
                    //    Id = v.Id,
                    //    Type = existingType.Name,
                    //    Value = v.Value
                    //}).ToList();

                    //return (typeDto, valueDtos);
                    throw new Exception("All attribute values already exist for this type. No new values added.");
                }

                // 5. Add only new values
                var newAttributeValues = newValuesToAdd.Select(v => new AttributeValue
                {
                    AttributeTypeId = existingType.Id,
                    Value = v
                }).ToList();

                await _repo.AddRangeAsync(newAttributeValues);
                await _repo.SaveChangesAsync();

                // 6. Return the updated full list of values (existing + new)
                var updatedValues = existingValues.Concat(newAttributeValues).ToList();

                var updatedValueDtos = updatedValues.Select(v => new AttributeValueDto
                {
                    Id = v.Id,
                    Type = existingType.Name,
                    Value = v.Value
                }).ToList();

                var updatedTypeDto = new AttributeTypeDto { Id = existingType.Id, Name = existingType.Name };

                return (updatedTypeDto, updatedValueDtos);
            }
        }

    }
}
