using Application.DTOs.Ticket;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Interface
{
    public interface ITicketService
    {
        Task<TicketResponseDto> CreateTicket(CreateTicketDto Dto);
        Task<IEnumerable<TicketResponseDto>> GetTickets();

        Task<TicketResponseDto?> GetTicketById(int id);

        Task<TicketResponseDto> UpdateTicket(int id, UpdateTicketDto dto);

        Task DeleteTicket(int id);

        Task<TicketResponseDto?> AssignTicket(int ticketId, string agentId);

        Task<TicketResponseDto?> ChangeStatus(int ticketId, TicketStatus status);
    }
}
