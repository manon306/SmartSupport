using Application.Features.Ticket.Queries.GetTickets;
using FluentValidation;

namespace Application.Validators
{
    public class GetTicketsQueryValidator
        : AbstractValidator<GetTicketsQuery>
    {
        public GetTicketsQueryValidator()
        {
            RuleFor(x => x.Filter.Page)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.Filter.PageSize)
                .InclusiveBetween(1, 100);
        }
    }
}