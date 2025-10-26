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
        private readonly IProductInventoryRepository _inventoryRepo;
        private readonly IProductImageRepository _imageRepo;

        public ProductService(
            IProductRepository productRepo,
            IAttributeValueRepository attributeRepo,
            IProductInventoryRepository inventoryRepo,
            IProductImageRepository imageRepo)
        {
            _productRepo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));
            _attributeRepo = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
            _inventoryRepo = inventoryRepo ?? throw new ArgumentNullException(nameof(inventoryRepo));
            _imageRepo = imageRepo ?? throw new ArgumentNullException(nameof(imageRepo));
        }

        public async Task<PagedResultDto<ProductDto>> GetAllAsync(
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            // Build the base query with includes
            var baseQuery = _productRepo.Query()
                .Include(p => p.Brand)
                .Include(p => p.Model)
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.AttributeValue)
                    .ThenInclude(av => av.AttributeType)
                .Include(p => p.ProductInventories)
                .Include(p => p.ProductImages)
                    .ThenInclude(pi => pi.ProductImageAttributeValues)
                    .ThenInclude(piav => piav.AttributeValue);

            // Apply search filter to the base query
            IQueryable<Product> filteredQuery = baseQuery; // Explicitly type as IQueryable<Product>

            if (!string.IsNullOrEmpty(searchTerm))
            {
                filteredQuery = filteredQuery.Where(p =>
                    p.SKU.Contains(searchTerm) ||
                    p.Brand.Name.Contains(searchTerm) ||
                    p.Model.Name.Contains(searchTerm));
            }

            // Count total before pagination
            var totalCount = await filteredQuery.CountAsync();

            // Apply pagination
            var products = await filteredQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map to DTO (rest of your code remains the same)
            var productDtos = products.Select(p => new ProductDto
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
                    Id = img.Id,
                    ProductId = img.ProductId,
                    ImageUrl = img.ImageUrl,
                    IsDefault = img.IsDefault,
                    AttributeValueIds = img.ProductImageAttributeValues?
                        .Select(piav => piav.AttributeValueId)
                        .ToList() ?? new List<int>()
                }).ToList(),
                DefaultImageUrl = p.ProductImages.FirstOrDefault(i => i.IsDefault)?.ImageUrl
                      ?? p.ProductImages.FirstOrDefault()?.ImageUrl
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
            var p = await _productRepo.Query()
                .Include(p => p.Brand)
                .Include(p => p.Model)
                .Include(p => p.ProductAttributes)
                    .ThenInclude(pa => pa.AttributeValue)
                    .ThenInclude(av => av.AttributeType)
                .Include(p => p.ProductInventories)
                .Include(p => p.ProductImages)
                    .ThenInclude(pi => pi.ProductImageAttributeValues)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null) return null;

            return new ProductDto
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
                    Id = img.Id,
                    ProductId = img.ProductId,
                    ImageUrl = img.ImageUrl,
                    IsDefault = img.IsDefault,
                    AttributeValueIds = img.ProductImageAttributeValues?
                        .Select(piav => piav.AttributeValueId)
                        .ToList() ?? new List<int>()
                }).ToList(),
                DefaultImageUrl = p.ProductImages.FirstOrDefault(i => i.IsDefault)?.ImageUrl
                      ?? p.ProductImages.FirstOrDefault()?.ImageUrl
            };
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
        {
            var attributeValues = await _attributeRepo.GetByIdsAsync(dto.AttributeValueIds);

            var product = new Product
            {
                BrandId = dto.BrandId,
                ModelId = dto.ModelId,
                SKU = GenerateSKU(dto.ModelId, attributeValues)
            };

            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync(); // Save to get product.Id

            product.ProductAttributes = attributeValues.Select(av => new ProductAttribute
            {
                AttributeValueId = av.Id,
                ProductId = product.Id
            }).ToList();

            // Add Inventory
            var inventory = new ProductInventory
            {
                ProductId = product.Id,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                InventoryAttributeValues = attributeValues.Select(av => new InventoryAttributeValue
                {
                    AttributeValueId = av.Id
                }).ToList()
            };
            await _inventoryRepo.AddAsync(inventory);

            // Optional: Add basic image during creation (without variant linking)
            if (!string.IsNullOrEmpty(dto.ImageUrl))
            {
                var image = new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = dto.ImageUrl,
                    IsDefault = true
                };
                await _imageRepo.AddAsync(image);
            }

            await _productRepo.SaveChangesAsync();

            return await GetByIdAsync(product.Id) ?? throw new Exception("Failed to load created product");
        }

        // NEW METHOD: Add variant-specific images after product creation
        public async Task<ProductImageDto> AddVariantImageAsync(ProductImageCreateDto dto)
        {
            var product = await _productRepo.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new ArgumentException("Product not found");

            var image = new ProductImage
            {
                ProductId = dto.ProductId,
                ImageUrl = dto.ImageUrl,
                IsDefault = dto.IsDefault
            };

            // Link to specific appearance attributes (color, antenna, etc.)
            if (dto.AppearanceAttributeValueIds?.Any() == true)
            {
                image.ProductImageAttributeValues = dto.AppearanceAttributeValueIds
                    .Select(avId => new ProductImageAttributeValue
                    {
                        AttributeValueId = avId
                    }).ToList();
            }

            await _imageRepo.AddAsync(image);
            await _imageRepo.SaveChangesAsync();

            return new ProductImageDto
            {
                Id = image.Id,
                ProductId = image.ProductId,
                ImageUrl = image.ImageUrl,
                IsDefault = image.IsDefault,
                AttributeValueIds = image.ProductImageAttributeValues?
                    .Select(piav => piav.AttributeValueId)
                    .ToList() ?? new List<int>()
            };
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