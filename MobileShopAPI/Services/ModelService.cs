using MobileShopAPI.DTOs;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Services
{
    public class ModelService : IModelService
    {
        private readonly IModelRepository _modelRepository;

        public ModelService(IModelRepository modelRepository)
        {
            _modelRepository = modelRepository;
        }

        public async Task<List<ModelDto>> GetAllModelsAsync()
        {
            var models = await _modelRepository.GetAllAsync();
            return models.Select(m => new ModelDto
            {
                Id = m.Id,
                Name = m.Name,
                BrandId = m.BrandId,
                BrandName = m.Brand.Name
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
                BrandId = model.BrandId
            };
        }

        public async Task<ModelDto> CreateModelAsync(ModelDto modelDto)
        {
            var model = new Model
            {
                Name = modelDto.Name,
                BrandId = modelDto.BrandId
            };

            await _modelRepository.AddAsync(model);
            await _modelRepository.SaveChangesAsync();

            return new ModelDto
            {
                Id = model.Id,
                Name = model.Name,
                BrandId = model.BrandId
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

        public async Task<List<ModelWithProductsDto>> GetAllModelsWithProductsAsync()
        {
            var models = await _modelRepository.GetAllWithProductsAsync();

            return models.Select(m => new ModelWithProductsDto
            {
                Id = m.Id,
                Name = m.Name,
                BrandId = m.BrandId,
                Products = m.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    SKU = p.SKU,
                    BrandId = p.BrandId,
                    BrandName = p.Brand?.Name ?? "",
                    ModelId = p.ModelId,
                    ModelName = p.Model?.Name ?? "",
                    Attributes = p.ProductAttributes.Select(pa => new AttributeValueDto
                    {
                        Id = pa.AttributeValueId,
                        Type = pa.AttributeValue.AttributeType.Name,
                        Value = pa.AttributeValue.Value
                    }).ToList(),
                    StockQuantity = p.ProductInventories.FirstOrDefault()?.StockQuantity ?? 0,
                    Price = p.ProductInventories.FirstOrDefault()?.Price ?? 0,

                    Images = p.ProductImages.Select(img => new ProductImageDto
                    {
                        ProductId = img.ProductId,
                        ImageUrl = img.ImageUrl,
                        IsDefault = img.IsDefault,
                        AttributeValueIds = img.ProductImageAttributeValues.Select(iav => iav.AttributeValueId).ToList()
                    }).ToList()
                }).ToList()


            }).ToList();
        }
    }
}
