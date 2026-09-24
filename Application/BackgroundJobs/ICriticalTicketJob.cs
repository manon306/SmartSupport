namespace Application.BackgroundJobs
{
    public interface ICriticalTicketJob
    {
        Task NotifyAdminsAboutCriticalTicketsAsync();
    }
}