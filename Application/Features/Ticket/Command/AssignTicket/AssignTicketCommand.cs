using Application.DTOs.Ticket;
using MediatR;

namespace Application.Features.Ticket.Command.AssignTicket
{
    public class AssignTicketCommand : IRequest<TicketResponseDto?>
    {
        public int TicketId { get; set; }
        public string AgentId { get; set; }
    }
}
