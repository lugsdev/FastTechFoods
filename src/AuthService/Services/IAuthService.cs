using Common.DTOs;

namespace AuthService.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
        Task<UserDto?> RegisterAsync(RegisterRequestDto registerRequest);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<bool> ValidateTokenAsync(string token);
        string GenerateJwtToken(UserDto user);
    }
}

