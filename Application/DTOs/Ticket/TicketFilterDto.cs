using Domain.Enums;

namespace Application.DTOs.Ticket
{
    public class TicketFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public TicketStatus? Status { get; set; }
        public TicketPriority? Priority { get; set; }

        public string? Search { get; set; }
        public string? AssignedTo { get; set; }
    }
}