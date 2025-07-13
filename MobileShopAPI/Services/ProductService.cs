using MobileShopAPI.DTOs;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IAttributeValueRepository _attributeRepo;

        public ProductService(IProductRepository productRepo, IAttributeValueRepository attributeRepo)
        {
            _productRepo = productRepo;
            _attributeRepo = attributeRepo;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            var products = await _productRepo.GetAllAsync();
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                SKU = p.SKU,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                BrandId = p.BrandId,
                BrandName = p.Brand?.Name ?? "",
                ModelId = p.ModelId,
                ModelName = p.Model?.Name ?? "",
                Attributes = p.ProductAttributes.Select(pa => new AttributeValueDto
                {
                    Id = pa.AttributeValueId,
                    Type = pa.AttributeValue.AttributeType.Name,
                    Value = pa.AttributeValue.Value
                }).ToList()
            }).ToList();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var p = await _productRepo.GetByIdAsync(id);
            if (p == null) return null;

            return new ProductDto
            {
                Id = p.Id,
                SKU = p.SKU,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                BrandId = p.BrandId,
                BrandName = p.Brand?.Name ?? "",
                ModelId = p.ModelId,
                ModelName = p.Model?.Name ?? "",
                Attributes = p.ProductAttributes.Select(pa => new AttributeValueDto
                {
                    Id = pa.AttributeValueId,
                    Type = pa.AttributeValue.AttributeType.Name,
                    Value = pa.AttributeValue.Value
                }).ToList()
            };
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
        {
            var attributeValues = await _attributeRepo.GetByIdsAsync(dto.AttributeValueIds);

            var product = new Product
            {
                BrandId = dto.BrandId,
                ModelId = dto.ModelId,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                SKU = GenerateSKU(dto.ModelId, attributeValues)
            };

            await _productRepo.AddAsync(product);

            // Add attributes after product is created
            product.ProductAttributes = attributeValues.Select(av => new ProductAttribute
            {
                AttributeValueId = av.Id,
                ProductId = product.Id // Set foreign key directly
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
            var parts = attributes.Select(a => a.Value);
            return $"M{modelId}-" + string.Join("-", parts);
        }
    }
}
