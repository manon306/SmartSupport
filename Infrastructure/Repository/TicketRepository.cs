using Application.DTOs.Ticket;
using Application.interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.ApplicationDBContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class TicketRepository(DBContext DB) : ITicketRepository
    {
        private readonly DBContext _DB = DB;
        public async Task<Ticket> CreateTicket(Ticket ticket)
        {
            _DB.Tickets.Add(ticket);
            await _DB.SaveChangesAsync();

            return ticket;

        }
        public async Task<(IEnumerable<Ticket> Items, int TotalCount)> GetTickets(TicketFilterDto filter,string? userId,string role)
        {
            var query = _DB.Tickets.AsQueryable();

            if (role == Roles.Employee)
            {
                query = query.Where(t => t.CreatedById == userId);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(t => t.Status == filter.Status.Value);
            }

            if (filter.Priority.HasValue)
            {
                query = query.Where(t => t.Priority == filter.Priority.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(t =>
                    t.Title.Contains(filter.Search) ||
                    t.Description.Contains(filter.Search));
            }

            if (!string.IsNullOrWhiteSpace(filter.AssignedTo))
            {
                query = query.Where(t =>
                    t.AssignedToId == filter.AssignedTo);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task<Ticket?> GetTicketById(int id)
        {
            return await _DB.Tickets.FirstOrDefaultAsync(a=>a.Id == id);
        }
        public async Task DeleteTicket(Ticket ticket)
        {
            _DB.Tickets.Remove(ticket);
            await _DB.SaveChangesAsync();
        }
        public async Task<bool> Exists(int ticketId)
        {
            bool exist = await _DB.Tickets.AnyAsync(a => a.Id == ticketId);
            return exist;
        }
        public async Task<Ticket> UpdateTicket(Ticket ticket)
        {
            _DB.Tickets.Update(ticket);
            await _DB.SaveChangesAsync();
            return ticket;
        }
        public async Task<Ticket?> ChangeStatus(int ticketId, TicketStatus status)
        {
            var ticket = await _DB.Tickets.FirstOrDefaultAsync(a => a.Id == ticketId);
            if (ticket != null)
            {
                ticket.Status = status;
                await _DB.SaveChangesAsync();
            }
            return ticket;
        }
        public async Task<Ticket?> AssignTicket(int ticketId, string agentId)
        {
            var ticket = await _DB.Tickets.FirstOrDefaultAsync(a => a.Id == ticketId);
            if (ticket != null)
            {
                ticket.AssignedToId = agentId;
                await _DB.SaveChangesAsync();
            }
            return ticket;
        }
    }
}
