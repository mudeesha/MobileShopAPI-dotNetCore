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

        private CartDto MapToDto(Cart cart)
        {
            var dto = new CartDto();
            
            foreach (var item in cart.Items)
            {
                dto.Items.Add(new CartItemDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    ProductName = item.Product?.Model?.Name ?? "Unknown Product",
                    ImageUrl = item.Product?.ProductImageAssignments?.FirstOrDefault()?.ProductImage?.ImageUrl
                });
            }

            dto.Total = dto.Items.Sum(i => i.Price * i.Quantity);
            
            return dto;
        }
    }
}