using MobileShopAPI.DTOs;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<CartDto> GetCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        
            if (cart == null)
            {
                cart = await _cartRepository.CreateCartAsync(userId);
                return new CartDto { Items = new List<CartItemDto>(), Total = 0 };
            }

            return MapToDto(cart);
        }

        public async Task<CartDto> AddToCartAsync(string userId, AddToCartDto dto)
        {
            // Get or create cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = await _cartRepository.CreateCartAsync(userId);
            }

            // Get product
            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found");

            if (product.StockQuantity < dto.Quantity)
                throw new InvalidOperationException($"Not enough stock. Available: {product.StockQuantity}");

            // Check if item already exists in cart
            var existingItem = await _cartRepository.GetCartItemAsync(cart.Id, dto.ProductId);
            
            if (existingItem != null)
            {
                // Update quantity
                existingItem.Quantity += dto.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
                await _cartRepository.UpdateCartItemAsync(existingItem);
            }
            else
            {
                // Add new item
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    Price = product.Price // Snapshot current price
                };
                await _cartRepository.AddCartItemAsync(cartItem);
            }

            await _cartRepository.SaveChangesAsync();
            
            // Return updated cart
            return await GetCartAsync(userId);
        }

        public async Task<CartDto> UpdateCartItemAsync(string userId, int productId, UpdateCartItemDto dto)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new KeyNotFoundException("Cart not found");

            var item = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (item == null)
                throw new KeyNotFoundException($"Item not found in cart");

            if (dto.Quantity <= 0)
            {
                return await RemoveFromCartAsync(userId, productId);
            }

            // Check stock
            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null && product.StockQuantity < dto.Quantity)
                throw new InvalidOperationException($"Not enough stock. Available: {product.StockQuantity}");

            item.Quantity = dto.Quantity;
            item.UpdatedAt = DateTime.UtcNow;
            
            await _cartRepository.UpdateCartItemAsync(item);
            await _cartRepository.SaveChangesAsync();

            return await GetCartAsync(userId);
        }

        public async Task<CartDto> RemoveFromCartAsync(string userId, int productId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new KeyNotFoundException("Cart not found");

            var item = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (item != null)
            {
                await _cartRepository.RemoveCartItemAsync(item);
                await _cartRepository.SaveChangesAsync();
            }

            return await GetCartAsync(userId);
        }

        public async Task<CartDto> ClearCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart != null)
            {
                await _cartRepository.ClearCartAsync(cart.Id);
                await _cartRepository.SaveChangesAsync();
            }

            return await GetCartAsync(userId);
        }
        
        private string? GetProductImageUrl(Product? product)
        {
            if (product == null || product.ProductImageAssignments == null)
                return null;

            var defaultImage = product.ProductImageAssignments
                .FirstOrDefault(pia => pia.IsDefault)?.ProductImage?.ImageUrl;
        
            return defaultImage ?? product.ProductImageAssignments
                .FirstOrDefault()?.ProductImage?.ImageUrl;
        }
        
        private List<AttributeValueDto>? GetProductAttributes(Product? product)
        {
            if (product?.ProductAttributes == null || !product.ProductAttributes.Any())
                return null;

            return product.ProductAttributes
                .Where(pa => pa.AttributeValue != null)
                .Select(pa => new AttributeValueDto
                {
                    Id = pa.AttributeValue.Id,
                    Type = pa.AttributeValue.AttributeType?.Name ?? "Unknown",
                    Value = pa.AttributeValue.Value
                }).ToList();
        }
        
        private string? GetAttributeSummary(Product? product)
        {
            var attributes = GetProductAttributes(product);
            if (attributes == null || !attributes.Any())
                return null;

            return string.Join(", ", attributes.Select(a => $"{a.Type}: {a.Value}"));
        }

        private CartDto MapToDto(Cart cart)
        {
            var cartDto = new CartDto
            {
                Items = cart.Items.Select(ci => new CartItemDto
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Price,
                    ProductName = ci.Product?.Model?.Name ?? "Unknown Product",
                    ImageUrl = GetProductImageUrl(ci.Product),
                    Attributes = GetProductAttributes(ci.Product),
                    AttributeSummary = GetAttributeSummary(ci.Product)
                }).ToList()
            };

            cartDto.Total = cartDto.Items.Sum(item => item.Price * item.Quantity);
            return cartDto;
        }
    }
}