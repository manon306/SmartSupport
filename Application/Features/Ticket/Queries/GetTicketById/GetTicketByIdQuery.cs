using Application.DTOs.Ticket;
using MediatR;

namespace Application.Features.Ticket.Queries.GetTicketById
{
    public class GetTicketByIdQuery :IRequest<TicketResponseDto?>
    {
        public int Id { get; set; }
    }
}
