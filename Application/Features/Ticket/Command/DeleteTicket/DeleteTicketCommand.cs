using MediatR;

namespace Application.Features.Ticket.Command.DeleteTicket
{
    public class DeleteTicketCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
