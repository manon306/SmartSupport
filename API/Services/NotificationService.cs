using API.Hubs;
using Application.Services.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace API.Services
{
    public class NotificationService(
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationService> logger)
        : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;
        private readonly ILogger<NotificationService> _logger = logger;

        public async Task NotifyUserAsync(
            string userId,
            string message)
        {
            _logger.LogInformation("Sending SignalR notification to user {UserId}.", userId);
            await _hubContext
                .Clients
                .User(userId)
                .SendAsync("ReceiveNotification", message);
        }
    }
}