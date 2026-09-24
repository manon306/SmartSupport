using Application.BackgroundJobs;
using Application.Services.Interface;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.ApplicationDBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.BackgroundJobs
{
    public class CriticalTicketJob(
        DBContext dbContext,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService,
        ILogger<CriticalTicketJob> logger)
        : ICriticalTicketJob
    {
        private readonly DBContext _dbContext = dbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly INotificationService _notificationService = notificationService;
        private readonly ILogger<CriticalTicketJob> _logger = logger;

        public async Task NotifyAdminsAboutCriticalTicketsAsync()
        {
            var criticalTickets = await _dbContext.Tickets
                .Where(t =>
                    t.Priority == TicketPriority.Critical &&
                    t.Status != TicketStatus.Closed)
                .ToListAsync();

            if (!criticalTickets.Any())
            {
                return;
            }
            
            _logger.LogInformation("Found {Count} critical unresolved tickets. Triggering notifications.", criticalTickets.Count);

            var admins = await _userManager
                .GetUsersInRoleAsync("Admin");

            foreach (var admin in admins)
            {
                await _notificationService.NotifyUserAsync(
                    admin.Id,
                    $"There are {criticalTickets.Count} unresolved critical tickets.");
            }
        }
    }
}