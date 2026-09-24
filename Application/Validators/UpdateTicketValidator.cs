using Application.Features.Ticket.Command.UpdateTicket;
using FluentValidation;

namespace Application.Validators
{
    public class UpdateTicketValidator : AbstractValidator<UpdateTicketCommand>
    {
        public UpdateTicketValidator()
        {
            RuleFor(x => x.Dto.Title)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Dto.Description)
                .NotEmpty()
                .MinimumLength(10);
        }
    }
}