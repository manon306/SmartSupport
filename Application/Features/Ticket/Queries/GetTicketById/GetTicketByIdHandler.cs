using Application.DTOs.Ticket;
using Application.interfaces;
using Application.Services.Interface;
using AutoMapper;
using Domain.Constants;
using MediatR;

namespace Application.Features.Ticket.Queries.GetTicketById
{
    public class GetTicketByIdHandler(
        ITicketRepository Repo,
        IMapper mapper,
        ITicketService TicketService)
        : IRequestHandler<GetTicketByIdQuery, TicketResponseDto?>
    {
        private readonly ITicketRepository _Repo = Repo;
        private readonly IMapper _Mapper = mapper;
        private readonly ITicketService _TicketService = TicketService;

        public async Task<TicketResponseDto?> Handle(
            GetTicketByIdQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _TicketService.GetCurrentUserId()
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var role = _TicketService.GetCurrentUserRole();

            var ticket = await _Repo.GetTicketById(request.Id);

            if (ticket == null)
            {
                return null;
            }

            if (role == Roles.Employee && ticket.CreatedById != userId)
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to access this ticket.");
            }

            return _Mapper.Map<TicketResponseDto>(ticket);
        }
    }
}