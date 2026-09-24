using Application.DTOs.Ticket;
using MediatR;

namespace Application.Features.Ticket.Command.CreateTicket
{
    public class CreateTicketCommand : IRequest<TicketResponseDto>
    {
        public CreateTicketDto Dto { get; set; }
    }
}
