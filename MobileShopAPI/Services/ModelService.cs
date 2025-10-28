using MobileShopAPI.DTOs;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;
using AutoMapper;

namespace MobileShopAPI.Services
{
    public class ModelService : IModelService
    {
        private readonly IModelRepository _modelRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public ModelService(IModelRepository modelRepository, IBrandRepository brandRepository, IMapper mapper)
        {
            _modelRepository = modelRepository;
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        public async Task<List<ModelDto>> GetAllModelsAsync()
        {
            var models = await _modelRepository.GetAllAsync();
            return models.Select(m => new ModelDto
            {
                Id = m.Id,
                Name = m.Name,
                BrandId = m.BrandId,
                BrandName = m.Brand?.Name ?? ""
            }).ToList();
        }

        public async Task<ModelDto?> GetModelByIdAsync(int id)
        {
            var model = await _modelRepository.GetByIdAsync(id);
            if (model == null) return null;

            return new ModelDto
            {
                Id = model.Id,
                Name = model.Name,
                BrandId = model.BrandId,
                BrandName = model.Brand?.Name ?? ""
            };
        }

        // ✅ Uses ModelCreateDto for input (create operation)
        public async Task<ModelDto> CreateModelAsync(ModelCreateDto modelDto)
        {
            var model = new Model
            {
                Name = modelDto.Name,
                BrandId = modelDto.BrandId
            };

            await _modelRepository.AddAsync(model);
            await _modelRepository.SaveChangesAsync();

            // Load the model with brand information for response
            var modelWithBrand = await _modelRepository.GetByIdAsync(model.Id);
            
            // ✅ Returns ModelDto for output (with BrandName)
            return new ModelDto
            {
                Id = model.Id,
                Name = model.Name,
                BrandId = model.BrandId,
                BrandName = modelWithBrand?.Brand?.Name ?? ""
            };
        }

        public async Task<bool> DeleteModelAsync(int id)
        {
            var model = await _modelRepository.GetByIdAsync(id);
            if (model == null) return false;

            await _modelRepository.DeleteAsync(model);
            await _modelRepository.SaveChangesAsync();

            return true;
        }
        
        public async Task<ModelDto> UpdateAsync(int id, UpdateModelDto dto)
        {
            var existingModel = await _modelRepository.GetByIdAsync(id);
            if (existingModel == null)
                throw new KeyNotFoundException($"Model with ID {id} not found.");

            var brandExists = await _brandRepository.GetByIdAsync(dto.BrandId);
            if (brandExists == null)
                throw new KeyNotFoundException($"Brand with ID {dto.BrandId} not found.");

            existingModel.Name = dto.Name;
            existingModel.BrandId = dto.BrandId;

            await _modelRepository.UpdateAsync(existingModel);
            
            return new ModelDto
            {
                Id = existingModel.Id,
                Name = existingModel.Name,
                BrandId = existingModel.BrandId,
                BrandName = brandExists.Name
            };
        }
    }
}