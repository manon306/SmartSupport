using Domain.Enums;
using System.ComponentModel.DataAnnotations;
namespace Application.DTOs.Ticket
{
    public class UpdateTicketDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public TicketPriority Priority { get; set; }
    }
}
