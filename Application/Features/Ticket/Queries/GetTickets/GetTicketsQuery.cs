using Application.DTOs.Ticket;
using MediatR;

namespace Application.Features.Ticket.Queries.GetTickets
{
    public record GetTicketsQuery(TicketFilterDto Filter)
        : IRequest<PagedResultDto<TicketResponseDto>>;
}