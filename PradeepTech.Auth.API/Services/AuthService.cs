using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PradeepTech.Auth.API.DTOs;
using PradeepTech.Auth.API.Models;
using System.Security.Claims;

namespace PradeepTech.Auth.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<ApplicationRole> _roleManager;

        private readonly IJwtService _jwtService;

        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IJwtService jwtService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, JwtTokenDto Token)> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null || !user.IsActive)
                    return (false, "Invalid email or password.", null);

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);
                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                        return (false, "Account is locked out.", null);
                    if (result.RequiresTwoFactor)
                        return (false, "Two-factor authentication required.", null);

                    return (false, "Invalid email or password.", null);
                }

                // Update last login
                user.LastLogin = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Generate JWT token
                var token = await _jwtService.GenerateTokenAsync(user);

                _logger.LogInformation($"User {user.Email} logged in successfully");

                return (true, "Login successful.", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", loginDto.Email);
                return (false, "An error occurred during login.", null);
            }
        }

        public async Task<(bool Success, string Message, UserDto User)> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                    return (false, "User with this email already exists.", null);

                var user = new ApplicationUser
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    UserName = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                    EmailConfirmed = true // Set based on your requirements
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, errors, null);
                }

                // Assign default role
                await _userManager.AddToRoleAsync(user, "User");

                var userDto = await MapToUserDto(user);

                _logger.LogInformation($"New user registered: {user.Email}");

                return (true, "User registered successfully.", userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", registerDto.Email);
                return (false, "An error occurred during registration.", null);
            }
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation($"User {userId} logged out");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {UserId}", userId);
                return false;
            }
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return (false, "User not found.");

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, errors);
                }

                return (true, "Password changed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return (false, "An error occurred while changing password.");
            }
        }

        public async Task<(bool Success, string Message)> AssignRoleAsync(AssignRoleDto assignRoleDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);
                if (user == null)
                    return (false, "User not found.");

                var roleExists = await _roleManager.RoleExistsAsync(assignRoleDto.RoleName);
                if (!roleExists)
                    return (false, "Role does not exist.");

                var result = await _userManager.AddToRoleAsync(user, assignRoleDto.RoleName);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, errors);
                }

                return (true, "Role assigned successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {Role} to user {UserId}", assignRoleDto.RoleName, assignRoleDto.UserId);
                return (false, "An error occurred while assigning role.");
            }
        }

        public async Task<(bool Success, string Message)> RemoveRoleAsync(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return (false, "User not found.");

                var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, errors);
                }

                return (true, "Role removed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {Role} from user {UserId}", roleName, userId);
                return (false, "An error occurred while removing role.");
            }
        }

        public async Task<UserDto> GetUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return await MapToUserDto(user);
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(await MapToUserDto(user));
            }

            return userDtos;
        }

        public async Task<(bool Success, string Message)> AddClaimAsync(string userId, string claimType, string claimValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return (false, "User not found.");

                var result = await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, errors);
                }

                return (true, "Claim added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding claim to user {UserId}", userId);
                return (false, "An error occurred while adding claim.");
            }
        }

        public async Task<(bool Success, string Message)> RemoveClaimAsync(string userId, string claimType)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return (false, "User not found.");

                var claims = await _userManager.GetClaimsAsync(user);
                var claimToRemove = claims.FirstOrDefault(c => c.Type == claimType);

                if (claimToRemove == null)
                    return (false, "Claim not found.");

                var result = await _userManager.RemoveClaimAsync(user, claimToRemove);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, errors);
                }

                return (true, "Claim removed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing claim from user {UserId}", userId);
                return (false, "An error occurred while removing claim.");
            }
        }

        public async Task<JwtTokenDto> RefreshTokenAsync(string refreshToken)
        {
            return await _jwtService.RefreshTokenAsync(refreshToken);
        }

        private async Task<UserDto> MapToUserDto(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                DateCreated = user.DateCreated,
                LastLogin = user.LastLogin,
                Roles = roles.ToList(),
                Claims = claims.Select(c => $"{c.Type}: {c.Value}").ToList()
            };
        }
    }
}