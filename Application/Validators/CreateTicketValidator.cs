using Application.Features.Ticket.Command.CreateTicket;
using FluentValidation;

namespace Application.Validators
{
    public class CreateTicketValidator : AbstractValidator<CreateTicketCommand>
    {
        public CreateTicketValidator()
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