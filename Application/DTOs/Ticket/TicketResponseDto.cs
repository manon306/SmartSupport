using Domain.Enums;
namespace Application.DTOs.Ticket
{
    public class TicketResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatedById { get; set; }
        public string? UpdatedById { get; set; }
        public string? AssignedToId { get; set; }
    }
}
