using Application.interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repository
{
    public class UserRepository(UserManager<ApplicationUser> userManager) : IUserRepository 
    { 
        private readonly UserManager<ApplicationUser> _userManager = userManager; 
        public async Task<ApplicationUser?> GetUserById(string userId) 
        { 
            return await _userManager.FindByIdAsync(userId); 
        } 
        public async Task<bool> IsUserInRole(string userId, string role) 
        { 
            var user = await _userManager.FindByIdAsync(userId); 
            if (user == null) { return false; }
            return await _userManager.IsInRoleAsync(user, role); 
        } 
    }
}
