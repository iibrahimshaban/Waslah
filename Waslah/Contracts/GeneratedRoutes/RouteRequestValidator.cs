namespace Waslah.Contracts.GeneratedRoutes;

public class RouteRequestValidator : AbstractValidator<RouteRequest>
{
    public RouteRequestValidator()
    {
        RuleFor(x => x.PrimaryId)
            .InclusiveBetween(10000, 400000)
            .WithMessage("PrimaryId must be between 10000 and 400000.");

        RuleFor(x => x.SecondaryId)
            .InclusiveBetween(10000, 400000)
            .WithMessage("SecondaryId must be between 10000 and 400000.");

        RuleFor(x => x.Time)
            .GreaterThan(0)
            .LessThanOrEqualTo(600)
            .WithMessage("Time must be greater than 0 and less than or equal to 600 minutes.");

        RuleFor(x => x.Distance)
            .GreaterThan(0)
            .LessThanOrEqualTo(100000)
            .WithMessage("Distance must be greater than 0 and less than or equal to 100,000 KM.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(500)
            .WithMessage("Price must be between 0 and 500 units.");

    }
}
