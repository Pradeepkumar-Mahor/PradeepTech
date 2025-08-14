using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PradeepTech.Auth.API.DTOs;
using PradeepTech.Auth.API.Services;

namespace PradeepTech.Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                token = result.Token.Token,
                expiration = result.Token.Expiration,
                refreshToken = result.Token.RefreshToken,
                user = result.Token.User
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, user = result.User });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID not found in token");

            var result = await _authService.LogoutAsync(userId);

            if (result)
                return Ok(new { message = "Logged out successfully" });

            return BadRequest(new { message = "Logout failed" });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID not found in token");

            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AssignRoleAsync(assignRoleDto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpDelete("remove-role/{userId}/{roleName}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            var result = await _authService.RemoveRoleAsync(userId, roleName);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("users")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _authService.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string userId)
        {
            // Users can only access their own data unless they're admin
            var currentUserId = User.FindFirst("userId")?.Value;
            var isAdmin = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");

            if (currentUserId != userId && !isAdmin)
                return Forbid();

            var user = await _authService.GetUserAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost("add-claim")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> AddClaim([FromBody] dynamic request)
        {
            string userId = request.userId;
            string claimType = request.claimType;
            string claimValue = request.claimValue;

            var result = await _authService.AddClaimAsync(userId, claimType, claimValue);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpDelete("remove-claim/{userId}/{claimType}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> RemoveClaim(string userId, string claimType)
        {
            var result = await _authService.RemoveClaimAsync(userId, claimType);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}