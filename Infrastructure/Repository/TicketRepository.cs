using Application.interfaces;
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
        public async Task<IEnumerable<Ticket>> GetTickets()
        {
            return await _DB.Tickets.ToListAsync();
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
