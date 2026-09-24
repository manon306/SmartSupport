using Application.DTOs;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.ApplicationDBContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController(
        UserManager<ApplicationUser> userManager,
        DBContext dbContext) : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly DBContext _dbContext = dbContext;

        /// <summary>
        /// Retrieves dashboard statistics including ticket counts and user count.
        /// </summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalTickets = await _dbContext.Tickets.CountAsync();
            var openTickets = await _dbContext.Tickets.CountAsync(t => t.Status == TicketStatus.Open);
            var inProgressTickets = await _dbContext.Tickets.CountAsync(t => t.Status == TicketStatus.InProgress);
            var resolvedTickets = await _dbContext.Tickets.CountAsync(t => t.Status == TicketStatus.Resolved);
            var closedTickets = await _dbContext.Tickets.CountAsync(t => t.Status == TicketStatus.Closed);
            var criticalTickets = await _dbContext.Tickets.CountAsync(t => t.Priority == TicketPriority.Critical);
            var unassignedTickets = await _dbContext.Tickets.CountAsync(t => t.AssignedToId == null);
            var totalUsers = await _dbContext.Users.CountAsync();

            return Ok(new DashboardStatsDto
            {
                TotalTickets = totalTickets,
                OpenTickets = openTickets,
                InProgressTickets = inProgressTickets,
                ResolvedTickets = resolvedTickets,
                ClosedTickets = closedTickets,
                CriticalTickets = criticalTickets,
                UnassignedTickets = unassignedTickets,
                TotalUsers = totalUsers
            });
        }

        /// <summary>
        /// Retrieves all users with their roles.
        /// </summary>
        [HttpGet("users")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    Roles = roles,
                    CreatedAt = user.CreatedAt
                });
            }

            return Ok(userDtos);
        }

        /// <summary>
        /// Retrieves a specific user by ID with their roles.
        /// </summary>
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Roles = roles,
                CreatedAt = user.CreatedAt
            });
        }

        /// <summary>
        /// Changes a user's role. Removes all existing roles and assigns the new one.
        /// </summary>
        [HttpPut("users/{userId}/role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeUserRole(string userId, [FromBody] ChangeRoleDto dto)
        {
            var validRoles = new[] { Roles.Employee, Roles.Agent, Roles.Admin };
            if (!validRoles.Contains(dto.Role))
            {
                return BadRequest(new { message = $"Invalid role. Must be one of: {string.Join(", ", validRoles)}" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, dto.Role);

            var updatedRoles = await _userManager.GetRolesAsync(user);
            return Ok(new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Roles = updatedRoles,
                CreatedAt = user.CreatedAt
            });
        }

        /// <summary>
        /// Retrieves all users in the Agent role (for ticket assignment dropdown).
        /// </summary>
        [HttpGet("agents")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAgents()
        {
            var agents = await _userManager.GetUsersInRoleAsync(Roles.Agent);
            var agentDtos = agents.Select(a => new UserDto
            {
                Id = a.Id,
                FullName = a.FullName,
                Email = a.Email!,
                Roles = new List<string> { Roles.Agent },
                CreatedAt = a.CreatedAt
            });

            return Ok(agentDtos);
        }
    }
}
