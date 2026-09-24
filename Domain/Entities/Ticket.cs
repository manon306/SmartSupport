using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatedById {  get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }

    }
}
