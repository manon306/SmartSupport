namespace Application.Services.Interface
{
    public interface INotificationService
    {
        Task NotifyUserAsync(
            string userId,
            string message);
    }
}