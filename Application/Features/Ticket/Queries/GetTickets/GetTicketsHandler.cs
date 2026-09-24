using Application.DTOs.Ticket;
using Application.interfaces;
using Application.Services.Interface;
using AutoMapper;
using MediatR;

namespace Application.Features.Ticket.Queries.GetTickets
{
    public class GetTicketsHandler(
        ITicketRepository Repo,
        IMapper mapper,
        ITicketService TicketService)
        : IRequestHandler<GetTicketsQuery, PagedResultDto<TicketResponseDto>>
    {
        private readonly ITicketRepository _Repo = Repo;
        private readonly IMapper _Mapper = mapper;
        private readonly ITicketService _TicketService = TicketService;

        public async Task<PagedResultDto<TicketResponseDto>> Handle(
            GetTicketsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _TicketService.GetCurrentUserId();
            var role = _TicketService.GetCurrentUserRole();

            if (userId == null)
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            var filter = request.Filter;

            if (filter.Page < 1)
            {
                filter.Page = 1;
            }

            if (filter.PageSize < 1)
            {
                filter.PageSize = 10;
            }

            if (filter.PageSize > 100)
            {
                filter.PageSize = 100;
            }

            var result = await _Repo.GetTickets(
                filter,
                userId,
                role);

            var items = _Mapper.Map<IEnumerable<TicketResponseDto>>(
                result.Items);

            var totalPages = (int)Math.Ceiling(
                result.TotalCount / (double)filter.PageSize);

            return new PagedResultDto<TicketResponseDto>
            {
                Items = items,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = totalPages
            };
        }
    }
}