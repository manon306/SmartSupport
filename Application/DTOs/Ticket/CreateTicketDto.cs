using Domain.Enums;
namespace Application.DTOs.Ticket
{
    public class CreateTicketDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketPriority Priority { get; set; }
    }
}
