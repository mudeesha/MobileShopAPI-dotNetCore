using MobileShopAPI.Models;

namespace MobileShopAPI.Repositories.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetCartByUserIdAsync(string userId);
    Task<CartItem?> GetCartItemAsync(int cartId, int productId);
    Task AddCartItemAsync(CartItem item);
    Task UpdateCartItemAsync(CartItem item);
    Task RemoveCartItemAsync(CartItem item);
    Task ClearCartAsync(int cartId);
    Task<Cart> CreateCartAsync(string userId);
    Task SaveChangesAsync();
}