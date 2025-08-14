using PradeepTech.Auth.API.DTOs;

namespace PradeepTech.Auth.API.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, JwtTokenDto Token)> LoginAsync(LoginDto loginDto);

        Task<(bool Success, string Message, UserDto User)> RegisterAsync(RegisterDto registerDto);

        Task<bool> LogoutAsync(string userId);

        Task<(bool Success, string Message)> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);

        Task<(bool Success, string Message)> AssignRoleAsync(AssignRoleDto assignRoleDto);

        Task<(bool Success, string Message)> RemoveRoleAsync(string userId, string roleName);

        Task<UserDto> GetUserAsync(string userId);

        Task<List<UserDto>> GetUsersAsync();

        Task<(bool Success, string Message)> AddClaimAsync(string userId, string claimType, string claimValue);

        Task<(bool Success, string Message)> RemoveClaimAsync(string userId, string claimType);

        Task<JwtTokenDto> RefreshTokenAsync(string refreshToken);
    }
}