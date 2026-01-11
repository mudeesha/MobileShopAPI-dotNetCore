using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MobileShopAPI.DTOs;
using MobileShopAPI.DTOs.Auth;
using MobileShopAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MobileShopAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
                return BadRequest(new { message = "User already exists." }); // Fixed: JSON object

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors }); // Fixed: JSON object

            await _userManager.AddToRoleAsync(user, "Customer");

            var token = await GenerateJwtToken(user);

            // Return both message and token
            return Ok(new
            {
                message = "User registered successfully",
                token = token
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" }); // Fixed: JSON object

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid credentials" }); // Fixed: JSON object

            var token = await GenerateJwtToken(user);
            
            // Get user roles for frontend
            var roles = await _userManager.GetRolesAsync(user);
            
            return Ok(new { 
                token = token,
                role = roles.FirstOrDefault() ?? "Customer",
                email = user.Email,
                fullName = user.UserName // Or whatever property stores full name
            });
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRoleToUser(AssignRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return NotFound(new { message = "User not found" }); // Fixed: JSON object

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                return BadRequest(new { message = "Role does not exist" }); // Fixed: JSON object

            var result = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors }); // Fixed: JSON object

            return Ok(new { message = $"User assigned to role '{dto.Role}' successfully." }); // Fixed: JSON object
        }
        
        [HttpPost("logout")]
        [Authorize] // This uses Microsoft.AspNetCore.Authorization
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Since JWT is stateless, we simply return success
                // The token will be invalidated on the client side
                // In future, you could add token blacklisting here if needed
            
                return Ok(new { 
                    success = true,
                    message = "Logout successful" 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false,
                    message = "Logout failed",
                    error = ex.Message 
                });
            }
        }
    }
}