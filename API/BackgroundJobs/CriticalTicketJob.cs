using Application.BackgroundJobs;
using Application.Services.Interface;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.ApplicationDBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.BackgroundJobs
{
    public class CriticalTicketJob(
        DBContext dbContext,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService)
        : ICriticalTicketJob
    {
        private readonly DBContext _dbContext = dbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly INotificationService _notificationService = notificationService;

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