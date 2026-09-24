using Application.DTOs.Ticket;
using Application.interfaces;
using Application.Services.Interface;
using AutoMapper;
using Domain.Enums;
using MediatR;

namespace Application.Features.Ticket.Command.CreateTicket
{
    public class CreateTicketHandler(
        ITicketRepository Repo,
        IMapper mapper,
        ITicketService TicketService)
        : IRequestHandler<CreateTicketCommand, TicketResponseDto>
    {
        private readonly ITicketRepository _Repo = Repo;
        private readonly IMapper _Mapper = mapper;
        private readonly ITicketService _TicketService = TicketService;

        public async Task<TicketResponseDto> Handle(
            CreateTicketCommand request,
            CancellationToken cancellationToken)
        {
            var ticket = _Mapper.Map<Domain.Entities.Ticket>(request.Dto);
            ticket.Status = TicketStatus.Open;
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;
            var userId = _TicketService.GetCurrentUserId() ?? throw new UnauthorizedAccessException("User is not authenticated.");
            ticket.CreatedById = userId;
            var result = await _Repo.CreateTicket(ticket);
            return _Mapper.Map<TicketResponseDto>(result);
        }
    }
}
