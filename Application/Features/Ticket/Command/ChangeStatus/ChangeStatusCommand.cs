using Application.DTOs.Ticket;
using Domain.Enums;
using MediatR;

namespace Application.Features.Ticket.Command.ChangeStatus
{
    public class ChangeStatusCommand : IRequest<TicketResponseDto?>
    {
        public int TicketId { get; set; }
        public TicketStatus Status { get; set; }
    }
}
