using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PradeepTech.Auth.API.DTOs;
using PradeepTech.Auth.API.Services;

namespace PradeepTech.Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst("userId")?.Value;
            var user = await _authService.GetUserAsync(userId);
            return Ok(user);
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetUsersAsync();
            return Ok(users);
        }

        [HttpPost("assign-role")]
        [Authorize(Policy = "CanManageUsers")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
        {
            var result = await _authService.AssignRoleAsync(model);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}