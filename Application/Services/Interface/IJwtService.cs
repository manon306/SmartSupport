namespace Application.Services.Interface
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string email, IEnumerable<string> roles);
        string GenerateRefreshToken();
    }
}
