using Application.DTOs;

namespace Application.Services.Interface
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshRequestDto dto);
        Task LogoutAsync(LogoutRequestDto dto);
    }
}
