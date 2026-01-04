// Services/CustomerModelService.cs
using MobileShopAPI.DTOs;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Services
{
    public class CustomerModelService : ICustomerModelService
    {
        private readonly ICustomerModelRepository _repository;

        public CustomerModelService(ICustomerModelRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ModelListingDto>> GetModelListingAsync()
        {
            var models = await _repository.GetModelsWithDetailsAsync();
            var result = new List<ModelListingDto>();

            foreach (var model in models)
            {
                var inStockProducts = model.Products?.Where(p => p.StockQuantity > 0).ToList() ?? new List<Product>();
                if (!inStockProducts.Any()) continue;
                
                // Get attribute options
                var attributeOptions = new Dictionary<string, List<string>>();
                foreach (var product in inStockProducts)
                {
                    if (product.ProductAttributes != null)
                    {
                        foreach (var pa in product.ProductAttributes)
                        {
                            if (pa.AttributeValue != null && pa.AttributeValue.AttributeType != null)
                            {
                                var type = pa.AttributeValue.AttributeType.Name;
                                var value = pa.AttributeValue.Value;
                                
                                if (!attributeOptions.ContainsKey(type))
                                    attributeOptions[type] = new List<string>();
                                
                                if (!attributeOptions[type].Contains(value))
                                    attributeOptions[type].Add(value);
                            }
                        }
                    }
                }
                
                // Map products
                var productVariants = inStockProducts.Select(p => MapToProductVariantDto(p)).ToList();

                result.Add(new ModelListingDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    BrandId = model.BrandId,
                    BrandName = model.Brand?.Name ?? "",
                    DefaultImageUrl = GetDefaultImage(inStockProducts),
                    MinPrice = inStockProducts.Min(p => p.Price),
                    MaxPrice = inStockProducts.Max(p => p.Price),
                    TotalStock = inStockProducts.Sum(p => p.StockQuantity),
                    AttributeOptions = attributeOptions,
                    Products = productVariants
                });
            }

            return result;
        }

        public async Task<ProductVariantDto?> GetProductByAttributeValueIdsAsync(
            int modelId, List<int> attributeValueIds)
        {
            // Simple validation
            if (attributeValueIds == null || !attributeValueIds.Any())
                throw new ArgumentException("At least one attribute value ID is required");

            if (attributeValueIds.Any(id => id <= 0))
                throw new ArgumentException("Attribute value IDs must be positive");

            if (attributeValueIds.Count != attributeValueIds.Distinct().Count())
                throw new ArgumentException("Duplicate IDs not allowed");

            var product = await _repository.GetProductByAttributeValueIdsAsync(modelId, attributeValueIds);

            if (product == null)
                throw new KeyNotFoundException($"No product found for model ID {modelId}");

            return MapToProductVariantDto(product);
        }

        private ProductVariantDto MapToProductVariantDto(Product product)
        {
            return new ProductVariantDto
            {
                Id = product.Id,
                SKU = product.SKU,
                Attributes = (product.ProductAttributes ?? new List<ProductAttribute>())
                    .Where(pa => pa.AttributeValue != null && pa.AttributeValue.AttributeType != null)
                    .Select(pa => new AttributeValueDto
                    {
                        Id = pa.AttributeValue!.Id,
                        Type = pa.AttributeValue!.AttributeType!.Name,
                        Value = pa.AttributeValue!.Value
                    })
                    .ToList(),
                StockQuantity = product.StockQuantity,
                Price = product.Price,
                DefaultImageUrl = GetProductImage(product),
                Images = (product.ProductImageAssignments ?? new List<ProductImageAssignment>())
                    .Where(pia => pia.ProductImage != null)
                    .Select(pia => pia.ProductImage!.ImageUrl)
                    .ToList()
            };
        }

        private string? GetDefaultImage(List<Product> products)
        {
            return products
                .SelectMany(p => p.ProductImageAssignments ?? new List<ProductImageAssignment>())
                .FirstOrDefault(pia => pia.ProductImage != null)?.ProductImage?.ImageUrl;
        }

        private string? GetProductImage(Product product)
        {
            return (product.ProductImageAssignments ?? new List<ProductImageAssignment>())
                .FirstOrDefault(pia => pia.ProductImage != null)?.ProductImage?.ImageUrl;
        }
    }
}