namespace Application.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int CriticalTickets { get; set; }
        public int UnassignedTickets { get; set; }
        public int TotalUsers { get; set; }
    }
}
