using Application.DTOs.Ticket;
using Application.interfaces;
using Application.Services.Interface;
using AutoMapper;
using Domain.Constants;
using MediatR;

namespace Application.Features.Ticket.Command.AssignTicket
{
    public class AssignTicketHandler(
        ITicketRepository Repo,
        IMapper mapper,
        ITicketService TicketService,
        IUserRepository userRepo)
        : IRequestHandler<AssignTicketCommand, TicketResponseDto?>
    {
        private readonly ITicketRepository _Repo = Repo;
        private readonly IMapper _Mapper = mapper;
        private readonly ITicketService _TicketService = TicketService;
        private readonly IUserRepository _UserRepo = userRepo;

        public async Task<TicketResponseDto?> Handle(
            AssignTicketCommand request,
            CancellationToken cancellationToken)
        {
            var role = _TicketService.GetCurrentUserRole();

            if (role != Roles.Admin)
            {
                throw new UnauthorizedAccessException(
                    "Only Admins can assign tickets.");
            }
            var ticket = await _Repo.GetTicketById(request.TicketId) ?? throw new KeyNotFoundException("Ticket not found.");

            var agent = await _UserRepo.GetUserById(request.AgentId);

            if (agent == null)
            {
                throw new KeyNotFoundException("Agent not found.");
            }

            var isAgent = await _UserRepo.IsUserInRole(request.AgentId, Roles.Agent);

            if (!isAgent)
            {
                throw new InvalidOperationException("The selected user is not an Agent.");
            }
            var result = await _Repo.AssignTicket(request.TicketId, request.AgentId);
            return _Mapper.Map<TicketResponseDto>(result);
        }
    }
}
