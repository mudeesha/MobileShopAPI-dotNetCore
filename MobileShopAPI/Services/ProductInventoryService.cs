using MobileShopAPI.DTOs;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Services
{
    public class ProductInventoryService : IProductInventoryService
    {
        private readonly IProductInventoryRepository _inventoryRepo;

        public ProductInventoryService(IProductInventoryRepository inventoryRepo)
        {
            _inventoryRepo = inventoryRepo;
        }

        public async Task<List<ProductInventoryDto>> GetByProductIdAsync(int productId)
        {
            var inventories = await _inventoryRepo.GetByProductIdAsync(productId);
            return inventories.Select(i => new ProductInventoryDto
            {
                ProductId = i.ProductId,
                StockQuantity = i.StockQuantity,
                Price = i.Price,
                AttributeValueIds = i.InventoryAttributeValues.Select(av => av.AttributeValueId).ToList()
            }).ToList();
        }

        public async Task<ProductInventoryDto?> GetExactMatchAsync(int productId, List<int> attributeValueIds)
        {
            var match = await _inventoryRepo.GetExactMatchAsync(productId, attributeValueIds);
            if (match == null) return null;

            return new ProductInventoryDto
            {
                ProductId = match.ProductId,
                StockQuantity = match.StockQuantity,
                Price = match.Price,
                AttributeValueIds = match.InventoryAttributeValues.Select(av => av.AttributeValueId).ToList()
            };
        }

        public async Task<ProductInventoryDto> CreateAsync(ProductInventoryCreateDto dto)
        {
            var inventory = new ProductInventory
            {
                ProductId = dto.ProductId,
                StockQuantity = dto.StockQuantity,
                Price = dto.Price,
                InventoryAttributeValues = dto.AttributeValueIds.Select(id => new InventoryAttributeValue
                {
                    AttributeValueId = id
                }).ToList()
            };

            await _inventoryRepo.AddAsync(inventory);
            await _inventoryRepo.SaveChangesAsync();

            return new ProductInventoryDto
            {
                ProductId = inventory.ProductId,
                StockQuantity = inventory.StockQuantity,
                Price = inventory.Price,
                AttributeValueIds = inventory.InventoryAttributeValues.Select(av => av.AttributeValueId).ToList()
            };
        }
    }
}
