using Application.DTOs.Ticket;
using Application.Exceptions;
using Application.interfaces;
using Application.Services.Interface;
using AutoMapper;
using Domain.Constants;
using Domain.Enums;
using MediatR;

namespace Application.Features.Ticket.Command.UpdateTicket
{
    public class UpdateTicketHandler(
        ITicketRepository Repo,
        IMapper mapper,
        ITicketService TicketService)
        : IRequestHandler<UpdateTicketCommand, TicketResponseDto>
    {
        private readonly ITicketRepository _Repo = Repo;
        private readonly IMapper _Mapper = mapper;
        private readonly ITicketService _TicketService = TicketService;

        public async Task<TicketResponseDto> Handle(
            UpdateTicketCommand request,
            CancellationToken cancellationToken)
        {
            var ticket = await _Repo.GetTicketById(request.Id) ?? throw new KeyNotFoundException("Ticket not found.");
            if (ticket.Status == TicketStatus.Closed)
            {
                throw new InvalidOperationException("Cannot update a closed ticket.");
            }
            var userId = _TicketService.GetCurrentUserId() ?? throw new UnauthorizedAccessException("User is not authenticated.");
            var role = _TicketService.GetCurrentUserRole();
            if (role == Roles.Employee && ticket.CreatedById != userId)
            {
                throw new ForbiddenException(
                    "You are not allowed to update this ticket.");
            }
            ticket.UpdatedById = userId;
            ticket.UpdatedAt = DateTime.UtcNow;
            ticket.Title = request.Dto.Title;
            ticket.Description = request.Dto.Description;

            if (role != Roles.Employee)
            {
                if (request.Dto.Priority == TicketPriority.Critical && ticket.AssignedToId == null)
                {
                    throw new InvalidOperationException(
                        "A Critical ticket must be assigned to an Agent.");
                }

                ticket.Priority = request.Dto.Priority;
            }
            var result = await _Repo.UpdateTicket(ticket);
            return _Mapper.Map<TicketResponseDto>(result);
        }
    }
}
