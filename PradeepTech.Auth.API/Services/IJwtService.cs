using PradeepTech.Auth.API.DTOs;
using PradeepTech.Auth.API.Models;

namespace PradeepTech.Auth.API.Services
{
    public interface IJwtService
    {
        Task<JwtTokenDto> GenerateTokenAsync(ApplicationUser user);

        Task<JwtTokenDto> RefreshTokenAsync(string refreshToken);

        Task<bool> ValidateTokenAsync(string token);
    }
}