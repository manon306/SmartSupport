using Application.DTOs.Ticket;
using Application.Exceptions;
using Application.interfaces;
using Application.Services.Interface;
using AutoMapper;
using Domain.Constants;
using Domain.Enums;
using MediatR;

namespace Application.Features.Ticket.Command.ChangeStatus
{
    public class ChangeStatusHandler(
        ITicketRepository Repo,
        IMapper mapper,
        ITicketService TicketService)
        : IRequestHandler<ChangeStatusCommand, TicketResponseDto?>
    {
        private readonly ITicketRepository _Repo = Repo;
        private readonly IMapper _Mapper = mapper;
        private readonly ITicketService _TicketService = TicketService;

        public async Task<TicketResponseDto?> Handle(
            ChangeStatusCommand request,
            CancellationToken cancellationToken)
        {
            var ticket = await _Repo.GetTicketById(request.TicketId) ?? throw new KeyNotFoundException("Ticket not found.");
            if (!_TicketService.IsValidStatusTransition(ticket.Status, request.Status))
            {
                throw new InvalidOperationException(
                    $"Cannot change ticket status from {ticket.Status} to {request.Status}.");
            }
            var userId = _TicketService.GetCurrentUserId() ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var role = _TicketService.GetCurrentUserRole();
            if (role == Roles.Employee)
            {
                throw new ForbiddenException(
                    "Employees cannot change ticket status.");
            }

            if (role == Roles.Agent && ticket.AssignedToId != userId)
            {
                throw new ForbiddenException(
                    "An Agent can only change the status of a ticket assigned to them.");
            }

            var result = await _Repo.ChangeStatus(request.TicketId, request.Status);
            return _Mapper.Map<TicketResponseDto>(result);
        }
    }
}
