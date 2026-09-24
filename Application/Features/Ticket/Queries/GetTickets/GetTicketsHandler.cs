using Application.DTOs.Ticket;
using Application.interfaces;
using Application.Services.Interface;
using AutoMapper;
using Domain.Constants;
using MediatR;

namespace Application.Features.Ticket.Queries.GetTickets
{
    public class GetTicketsHandler(
        ITicketRepository Repo,
        IMapper mapper,
        ITicketService TicketService)
        : IRequestHandler<GetTicketsQuery, IEnumerable<TicketResponseDto>>
    {
        private readonly ITicketRepository _Repo = Repo;
        private readonly IMapper _Mapper = mapper;
        private readonly ITicketService _TicketService = TicketService;

        public async Task<IEnumerable<TicketResponseDto>> Handle(
            GetTicketsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _TicketService.GetCurrentUserId();
            var role = _TicketService.GetCurrentUserRole();

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var tickets = await _Repo.GetTickets();

            if (role == Roles.Employee)
            {
                tickets = tickets.Where(t => t.CreatedById == userId);
            }

            return _Mapper.Map<IEnumerable<TicketResponseDto>>(tickets);
        }
    }
}
