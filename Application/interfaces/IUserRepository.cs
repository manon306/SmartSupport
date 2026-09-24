using Domain.Entities;

namespace Application.interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetUserById(string userId);
        Task<bool> IsUserInRole(string userId, string role);
    }
}
