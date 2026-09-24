using Application.DTOs.Ticket;
using Domain.Entities;
using Domain.Enums;

namespace Application.interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket> CreateTicket(Ticket ticket);
        Task<(IEnumerable<Ticket> Items, int TotalCount)> GetTickets(
    TicketFilterDto filter,
    string? userId,
    string role);

        Task<Ticket?> GetTicketById(int id);

        Task<Ticket> UpdateTicket(Ticket ticket);

        Task DeleteTicket(Ticket ticket);

        Task<Ticket?> AssignTicket(int ticketId, string agentId);

        Task<Ticket?> ChangeStatus(int ticketId, TicketStatus status);

        Task<bool> Exists(int ticketId);
    }
}
