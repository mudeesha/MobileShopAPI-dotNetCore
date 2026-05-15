using MobileShopAPI.DTOs;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MobileShopAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IAttributeValueRepository _attributeRepo;
        private readonly IProductImageRepository _imageRepo;
        private readonly IModelRepository _modelRepository;
        

        public ProductService(
            IProductRepository productRepo,
            IAttributeValueRepository attributeRepo,
            IProductImageRepository imageRepo,
            IModelRepository modelRepository)
        {
            _productRepo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));
            _attributeRepo = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
            _imageRepo = imageRepo ?? throw new ArgumentNullException(nameof(imageRepo));
            _modelRepository = modelRepository;
        }

        public async Task<PagedResultDto<ProductDto>> GetAllAsync(
            string? searchTerm = null,
            int pageNumber = 1, 
            int pageSize = 10)
        {
            var query = _productRepo.Query();
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p =>
                    p.SKU.Contains(searchTerm) ||
                    p.Model.Brand.Name.Contains(searchTerm) ||
                    p.Model.Name.Contains(searchTerm));
            }
            
            var totalCount = await query.CountAsync();
            
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                SKU = p.SKU,
                BrandId = p.Model.BrandId,
                BrandName = p.Model.Brand?.Name ?? "",
                ModelId = p.ModelId,
                ModelName = p.Model?.Name ?? "",
                Attributes = p.ProductAttributes.Select(pa => new AttributeValueDto
                {
                    Id = pa.AttributeValueId,
                    Type = pa.AttributeValue.AttributeType.Name,
                    Value = pa.AttributeValue.Value
                }).ToList(),
                StockQuantity = p.StockQuantity,
                Price = p.Price,
                Images = p.ProductImageAssignments.Select(pia => new ProductImageDto
                {
                    Id = pia.ProductImage.Id,
                    ImageUrl = pia.ProductImage.ImageUrl,
                    IsDefault = pia.IsDefault,
                }).ToList(),
                DefaultImageUrl = p.ProductImageAssignments
                                      .FirstOrDefault(pia => pia.IsDefault)?.ProductImage?.ImageUrl
                                  ?? p.ProductImageAssignments.FirstOrDefault()?.ProductImage?.ImageUrl
            }).ToList();

            return new PagedResultDto<ProductDto>
            {
                Items = productDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var p = await _productRepo.GetByIdAsync(id);
            if (p == null) return null;

            return new ProductDto
            {
                Id = p.Id,
                SKU = p.SKU,
                BrandId = p.Model.BrandId,
                BrandName = p.Model.Brand?.Name ?? "",
                ModelId = p.ModelId,
                ModelName = p.Model?.Name ?? "",
                Attributes = p.ProductAttributes.Select(pa => new AttributeValueDto
                {
                    Id = pa.AttributeValueId,
                    Type = pa.AttributeValue.AttributeType.Name,
                    Value = pa.AttributeValue.Value
                }).ToList(),
                StockQuantity = p.StockQuantity,
                Price = p.Price,
                Images = p.ProductImageAssignments.Select(pia => new ProductImageDto
                {
                    Id = pia.ProductImage.Id,
                    ImageUrl = pia.ProductImage.ImageUrl,
                    IsDefault = pia.IsDefault,
                }).ToList(),
                DefaultImageUrl = p.ProductImageAssignments
                                      .FirstOrDefault(pia => pia.IsDefault)?.ProductImage?.ImageUrl
                                  ?? p.ProductImageAssignments.FirstOrDefault()?.ProductImage?.ImageUrl
            };
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
        {
            var attributeValues = await _attributeRepo.GetByIdsAsync(dto.AttributeValueIds);
            
            ValidateAttributeValues(attributeValues);
            
            await ValidateUniqueProductAsync(dto.ModelId, attributeValues);

            var product = new Product
            {
                ModelId = dto.ModelId,
                SKU = GenerateSKU(dto.ModelId, attributeValues),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };

            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            product.ProductAttributes = attributeValues.Select(av => new ProductAttribute
            {
                AttributeValueId = av.Id,
                ProductId = product.Id
            }).ToList();
            
            await _productRepo.SaveChangesAsync();

            return await GetByIdAsync(product.Id) ?? throw new Exception("Failed to load created product");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return false;

            await _productRepo.DeleteAsync(product);
            await _productRepo.SaveChangesAsync();
            return true;
        }

        private string GenerateSKU(int modelId, List<AttributeValue> attributes)
        {
            var distinctAttributes = attributes
                .GroupBy(a => a.AttributeTypeId)
                .Select(g => g.First())
                .OrderBy(a => a.AttributeTypeId);

            var parts = distinctAttributes.Select(a => a.Value);
            return $"M{modelId}-" + string.Join("-", parts);
        }
        
        private void ValidateAttributeValues(List<AttributeValue> attributeValues)
        {
            var duplicateTypes = attributeValues
                .GroupBy(av => av.AttributeTypeId)
                .Where(g => g.Count() > 1)
                .Select(g => g.First().AttributeType?.Name)
                .ToList();

            if (duplicateTypes.Any())
            {
                throw new ArgumentException(
                    $"Product cannot have multiple values for the same attribute type. " +
                    $"Duplicate types: {string.Join(", ", duplicateTypes)}");
            }
        }
        
        private async Task ValidateUniqueProductAsync(int modelId, List<AttributeValue> attributeValues)
        {
            var existingProducts = await _productRepo.GetByModelIdAsync(modelId);
    
            foreach (var existingProduct in existingProducts)
            {
                var existingAttributeIds = existingProduct.ProductAttributes
                    .Select(pa => pa.AttributeValueId)
                    .OrderBy(id => id)
                    .ToList();
            
                var newAttributeIds = attributeValues
                    .Select(av => av.Id)
                    .OrderBy(id => id)
                    .ToList();
                
                if (existingAttributeIds.SequenceEqual(newAttributeIds))
                {
                    var attributeNames = string.Join(", ", attributeValues.Select(av => av.Value));
                    throw new ArgumentException(
                        $"A product with model ID {modelId} and attributes [{attributeNames}] already exists. " +
                        $"Existing product ID: {existingProduct.Id}");
                }
            }
        }
        
        public async Task<ProductDto> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var existingProduct = await _productRepo.GetByIdAsync(id);
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            
            var model = await _modelRepository.GetByIdAsync(dto.ModelId);
            if (model == null)
                throw new KeyNotFoundException($"Model with ID {dto.ModelId} not found.");
            
            var attributeValues = await _attributeRepo.GetByIdsAsync(dto.AttributeValueIds);
            ValidateAttributeValues(attributeValues);
            
            var allProductsForModel = await _productRepo.GetByModelIdAsync(dto.ModelId);
            var otherProducts = allProductsForModel.Where(p => p.Id != id).ToList();
            
            foreach (var existing in otherProducts)
            {
                var existingAttributeIds = existing.ProductAttributes
                    .Select(pa => pa.AttributeValueId)
                    .OrderBy(id => id)
                    .ToList();

                var newAttributeIds = attributeValues
                    .Select(av => av.Id)
                    .OrderBy(id => id)
                    .ToList();

                if (existingAttributeIds.SequenceEqual(newAttributeIds))
                {
                    var attributeNames = string.Join(", ", attributeValues.Select(av => av.Value));
                    throw new ArgumentException(
                        $"A product with model ID {dto.ModelId} and attributes [{attributeNames}] already exists. " +
                        $"Existing product ID: {existing.Id}");
                }
            }
            
            existingProduct.SKU = GenerateSKU(dto.ModelId, attributeValues);
            
            existingProduct.ModelId = dto.ModelId;
            existingProduct.Price = dto.Price;
            existingProduct.StockQuantity = dto.StockQuantity;
            existingProduct.ProductAttributes.Clear();
            existingProduct.ProductAttributes = attributeValues.Select(av => new ProductAttribute
            {
                ProductId = existingProduct.Id,
                AttributeValueId = av.Id
            }).ToList();

            await _productRepo.UpdateAsync(existingProduct);
            await _productRepo.SaveChangesAsync();
            
            return await GetByIdAsync(existingProduct.Id) ?? throw new Exception("Failed to load updated product");
        }
    }
}