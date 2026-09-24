using Application.DTOs.Ticket;
using MediatR;

namespace Application.Features.Ticket.Command.UpdateTicket
{
    public class UpdateTicketCommand : IRequest<TicketResponseDto>
    {
        public int Id { get; set; }
        public UpdateTicketDto Dto { get; set; }
    }
}
