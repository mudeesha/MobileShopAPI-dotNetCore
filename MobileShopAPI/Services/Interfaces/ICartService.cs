using MobileShopAPI.DTOs;
namespace MobileShopAPI.Services.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(string userId);
    Task<CartDto> AddToCartAsync(string userId, AddToCartDto dto);
    Task<CartDto> UpdateCartItemAsync(string userId, int productId, UpdateCartItemDto dto);
    Task<CartDto> RemoveFromCartAsync(string userId, int productId);
    Task<CartDto> ClearCartAsync(string userId); 
}