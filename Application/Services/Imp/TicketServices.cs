using Application.DTOs.Ticket;
using Application.interfaces;
using Application.Services.Interface;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Services.Imp
{
    public class TicketServices(ITicketRepository Repo, IMapper mapper,IHttpContextAccessor accessor, IUserRepository userRepo) : ITicketService
    {
        private readonly ITicketRepository _Repo = Repo;
        private readonly IMapper _Mapper = mapper;
        private readonly IHttpContextAccessor _accessor = accessor;
        private readonly IUserRepository _UserRepo = userRepo;
        public string? GetCurrentUserId()
        {
            return _accessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        public string? GetCurrentUserRole()
        {
            return _accessor.HttpContext?.User
                .FindFirst(ClaimTypes.Role)?.Value;
        }
        public bool IsValidStatusTransition(TicketStatus currentStatus,TicketStatus newStatus)
        {
            return currentStatus switch
            {
                TicketStatus.Open =>
                    newStatus == TicketStatus.InProgress,

                TicketStatus.InProgress =>
                    newStatus == TicketStatus.Resolved,

                TicketStatus.Resolved =>
                    newStatus == TicketStatus.Closed,

                TicketStatus.Closed =>
                    false,

                _ => false
            };
        }
        public async Task<TicketResponseDto> CreateTicket(CreateTicketDto Dto)
        {
            var ticket = _Mapper.Map<Ticket>(Dto);
            ticket.Status = TicketStatus.Open;
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;
            var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("User is not authenticated.");
            ticket.CreatedById = userId;
            var result = await _Repo.CreateTicket(ticket);
            return _Mapper.Map<TicketResponseDto>(result);

        }
        public async Task<TicketResponseDto?> GetTicketById(int id)
        {
            var userId = GetCurrentUserId() ?? throw new UnauthorizedAccessException("User is not authenticated.");
            var role = GetCurrentUserRole();

            var ticket = await _Repo.GetTicketById(id);

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

        //public async Task<IEnumerable<TicketResponseDto>> GetTickets()
        //{
        //    var userId = GetCurrentUserId();
        //    var role = GetCurrentUserRole();

        //    if (userId == null)
        //    {
        //        throw new UnauthorizedAccessException("User is not authenticated.");
        //    }

        //    var tickets = await _Repo.GetTickets();

        //    if (role == Roles.Employee)
        //    {
        //        tickets = tickets.Where(t => t.CreatedById == userId);
        //    }

        //    return _Mapper.Map<IEnumerable<TicketResponseDto>>(tickets);
        //}
        public async Task DeleteTicket(int id)
        {
            var role = GetCurrentUserRole();

            if (role != Roles.Admin)
            {
                throw new UnauthorizedAccessException(
                    "Only Admins can delete tickets.");
            }

            var ticket = await _Repo.GetTicketById(id)
                ?? throw new KeyNotFoundException("Ticket not found.");

            await _Repo.DeleteTicket(ticket);
        }
        public async Task<TicketResponseDto> UpdateTicket(int id, UpdateTicketDto dto)
        {
            var ticket = await _Repo.GetTicketById(id) ?? throw new KeyNotFoundException("Ticket not found.");
            if (ticket.Status == TicketStatus.Closed)
            {
                throw new InvalidOperationException("Cannot update a closed ticket.");
            }
            var userId = GetCurrentUserId()?? throw new UnauthorizedAccessException("User is not authenticated.");
            var role = GetCurrentUserRole();
            if (role == Roles.Employee && ticket.CreatedById != userId)
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to update this ticket.");
            }
            ticket.UpdatedById = userId;
            ticket.UpdatedAt = DateTime.UtcNow;
            ticket.Title = dto.Title;
            ticket.Description = dto.Description;

            if (role != Roles.Employee)
            {
                if (dto.Priority == TicketPriority.Critical && ticket.AssignedToId == null)
                {
                    throw new InvalidOperationException(
                        "A Critical ticket must be assigned to an Agent.");
                }

                ticket.Priority = dto.Priority;
            }
            var result = await _Repo.UpdateTicket(ticket);
            return _Mapper.Map<TicketResponseDto>(result);
        }
        public async Task<TicketResponseDto?> ChangeStatus(int ticketId, TicketStatus status)
        {
            var ticket = await _Repo.GetTicketById(ticketId)?? throw new KeyNotFoundException("Ticket not found.");
            if (!IsValidStatusTransition(ticket.Status, status))
            {
                throw new InvalidOperationException(
                    $"Cannot change ticket status from {ticket.Status} to {status}.");
            }
            var userId = GetCurrentUserId()?? throw new UnauthorizedAccessException("User is not authenticated.");

            var role = GetCurrentUserRole();
            if (role == Roles.Employee)
            {
                throw new UnauthorizedAccessException(
                    "Employees cannot change ticket status.");
            }
            if (role == Roles.Agent && ticket.AssignedToId != userId)
            {
                throw new UnauthorizedAccessException(
                    "An Agent can only change the status of a ticket assigned to them.");
            }
            
            var result = await _Repo.ChangeStatus(ticketId, status);
            return _Mapper.Map<TicketResponseDto>(result);
        }
        public async Task<TicketResponseDto?> AssignTicket(int ticketId, string agentId)
        {
            var role = GetCurrentUserRole();

            if (role != Roles.Admin)
            {
                throw new UnauthorizedAccessException(
                    "Only Admins can assign tickets.");
            }
            var ticket = await _Repo.GetTicketById(ticketId) ?? throw new KeyNotFoundException("Ticket not found.");

            var agent = await _UserRepo.GetUserById(agentId);

            if (agent == null)
            {
                throw new KeyNotFoundException("Agent not found.");
            }

            var isAgent = await _UserRepo.IsUserInRole(agentId, Roles.Agent);

            if (!isAgent)
            {
                throw new InvalidOperationException("The selected user is not an Agent.");
            }
            var result = await _Repo.AssignTicket(ticketId, agentId);
            return _Mapper.Map<TicketResponseDto>(result);
        }
    }
}
