using Microsoft.EntityFrameworkCore;
using MobileShopAPI.Data;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;

namespace MobileShopAPI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public async Task AddCartItemAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
        }

        public async Task UpdateCartItemAsync(CartItem item)
        {
            _context.CartItems.Update(item);
        }

        public async Task RemoveCartItemAsync(CartItem item)
        {
            _context.CartItems.Remove(item);
        }

        public async Task ClearCartAsync(int cartId)
        {
            var items = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();
            
            _context.CartItems.RemoveRange(items);
        }

        public async Task<Cart> CreateCartAsync(string userId)
        {
            var cart = new Cart { UserId = userId };
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}