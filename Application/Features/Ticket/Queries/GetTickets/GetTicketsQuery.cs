using Application.DTOs.Ticket;
using MediatR;

namespace Application.Features.Ticket.Queries.GetTickets
{
    public class GetTicketsQuery : IRequest<IEnumerable<TicketResponseDto>>
    {
    }
}
