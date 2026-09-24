using Application.DTOs;
using Application.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        /// <summary>
        /// Authenticates a user with email and password credentials.
        /// </summary>
        /// <param name="dto">The login credentials including email and password.</param>
        /// <returns>An authentication response containing the JWT access token and refresh token.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);
            return Ok(response);
        }

        /// <summary>
        /// Registers a new user account with the candidate role.
        /// </summary>
        /// <param name="dto">The registration details including full name, email, and password.</param>
        /// <returns>An authentication response containing the JWT access token and refresh token for the new user.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var response = await _authService.RegisterAsync(dto);
            return Ok(response);
        }

        /// <summary>
        /// Refreshes an expired access token using a valid refresh token.
        /// </summary>
        /// <param name="dto">The refresh token request containing the active refresh token.</param>
        /// <returns>An authentication response containing a newly issued JWT access token and refresh token.</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequestDto dto)
        {
            var response = await _authService.RefreshTokenAsync(dto);
            return Ok(response);
        }

        /// <summary>
        /// Revokes an active refresh token to log out the user.
        /// </summary>
        /// <param name="refreshToken">The logout request containing the refresh token to revoke.</param>
        /// <returns>A confirmation message indicating the user was logged out successfully.</returns>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto refreshToken)
        {
            await _authService.LogoutAsync(refreshToken);
            return Ok(new { message = "Logged out successfully." });
        }
    }
}
