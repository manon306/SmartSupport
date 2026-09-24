using Application.Services.Interface;
using Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.Imp
{
    public class JwtService(IOptions<JwtSettings> jwtSettings) : IJwtService
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public string GenerateToken(string userId, string email, IEnumerable<string> roles)
        {
            // Create claims for the token => info in the token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            // Create a symmetric security key using a secret key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key)
            );
            // Create signing credentials using the key and the HMAC SHA256 algorithm
            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );
            // Create the JWT token with claims, expiration, and signing credentials
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                signingCredentials: credentials
            );
            // Generate the token string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)
            );
        }
    }
}
