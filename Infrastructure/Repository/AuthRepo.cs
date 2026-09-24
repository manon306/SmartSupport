using Application.interfaces;
using Domain.Entities;
using Infrastructure.ApplicationDBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class AuthRepo(DBContext context, UserManager<ApplicationUser> userManager) : IAuthRepo
    {
        private readonly DBContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<ApplicationUser?> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new Exception("Invalid email or password.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user,password);
            if (!isPasswordValid)
            {
                throw new Exception("Invalid email or password.");
            }
            return user;
        }
        public async Task<bool> RegisterAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }
            return result.Succeeded;
        }
        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == token);
        }
        public async Task LogoutAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken) ?? throw new KeyNotFoundException("Refresh token not found.");
            if (storedToken.RevokedAt != null)
            {
                return;
            }
            storedToken.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
