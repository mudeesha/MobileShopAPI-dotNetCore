using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileShopAPI.DTOs;
using MobileShopAPI.Services.Interfaces;
using System.Security.Claims;

namespace MobileShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        private string GetUserId()
        {
            // Get ALL NameIdentifier claims
            var nameIdentifierClaims = User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .ToList();
    
            // Log for debugging
            _logger.LogInformation("Found {Count} NameIdentifier claims", nameIdentifierClaims.Count);
    
            foreach (var claim in nameIdentifierClaims)
            {
                _logger.LogInformation("NameIdentifier: {Value} (looks like email: {IsEmail})", 
                    claim.Value, claim.Value.Contains("@"));
            }
    
            // Prefer the one that's NOT an email (the GUID)
            var userIdClaim = nameIdentifierClaims.FirstOrDefault(c => !c.Value.Contains("@"));
    
            if (userIdClaim != null)
            {
                _logger.LogInformation("Using non-email NameIdentifier: {UserId}", userIdClaim.Value);
                return userIdClaim.Value;
            }
    
            // Fallback to first one
            var firstClaim = nameIdentifierClaims.FirstOrDefault();
            if (firstClaim != null)
            {
                _logger.LogWarning("Using email as UserId (no GUID found): {Email}", firstClaim.Value);
        
                // If it's an email, we need to find the actual user ID
                if (firstClaim.Value.Contains("@"))
                {
                    throw new UnauthorizedAccessException(
                        "Token contains email as NameIdentifier. Please login again for new token.");
                }
        
                return firstClaim.Value;
            }
    
            throw new UnauthorizedAccessException("User not authenticated");
        }
        
        [HttpGet("debug-claims")]
        [Authorize]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new 
            { 
                Type = c.Type, 
                Value = c.Value,
                ValueType = c.ValueType
            }).ToList();
    
            return Ok(new
            {
                UserIdFromNameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier),
                EmailFromEmail = User.FindFirstValue(ClaimTypes.Email),
                EmailFromNameIdentifier2 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                AllClaims = claims
            });
        }

        [HttpGet]
        [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.GetCartAsync(userId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("add")]
        [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.AddToCartAsync(userId, dto);
                return Ok(cart);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPut("{productId}")]
        [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCartItem(int productId, [FromBody] UpdateCartItemDto dto)
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.UpdateCartItemAsync(userId, productId, dto);
                return Ok(cart);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpDelete("{productId}")]
        [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.RemoveFromCartAsync(userId, productId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from cart");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.ClearCartAsync(userId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}