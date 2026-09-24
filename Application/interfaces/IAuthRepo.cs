using Domain.Entities;
namespace Application.interfaces
{
    public interface IAuthRepo
    {
        public Task<bool> RegisterAsync(ApplicationUser user, string password);
        Task<ApplicationUser?> LoginAsync(string email, string password);
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task LogoutAsync(string refreshToken);
    }
}
