using Application.interfaces;
using Application.Services.Interface;
using Domain.Constants;
using MediatR;

namespace Application.Features.Ticket.Command.DeleteTicket
{
    public class DeleteTicketHandler(
        ITicketRepository Repo,
        ITicketService TicketService)
        : IRequestHandler<DeleteTicketCommand, Unit>
    {
        private readonly ITicketRepository _Repo = Repo;
        private readonly ITicketService _TicketService = TicketService;

        public async Task<Unit> Handle(
            DeleteTicketCommand request,
            CancellationToken cancellationToken)
        {
            var role = _TicketService.GetCurrentUserRole();

            if (role != Roles.Admin)
            {
                throw new UnauthorizedAccessException(
                    "Only Admins can delete tickets.");
            }

            var ticket = await _Repo.GetTicketById(request.Id)
                ?? throw new KeyNotFoundException("Ticket not found.");

            await _Repo.DeleteTicket(ticket);

            return Unit.Value;
        }
    }
}
