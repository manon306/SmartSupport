using API.Hubs;
using Application.Services.Interface;
using Microsoft.AspNetCore.SignalR;

namespace API.Services
{
    public class NotificationService(
        IHubContext<NotificationHub> hubContext)
        : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;

        public async Task NotifyUserAsync(
            string userId,
            string message)
        {
            await _hubContext
                .Clients
                .User(userId)
                .SendAsync("ReceiveNotification", message);
        }
    }
}