using FluentValidation;

namespace DentalBooking.Application.Reports.Queries;

public class GetStatisticsValidator : AbstractValidator<GetStatisticsQuery>
{
    public GetStatisticsValidator()
    {
        RuleFor(x => x.FromUtc)
            .LessThanOrEqualTo(x => x.ToUtc)
            .When(x => x.FromUtc.HasValue && x.ToUtc.HasValue)
            .WithMessage("FromUtc must be earlier than ToUtc.");
    }
}
