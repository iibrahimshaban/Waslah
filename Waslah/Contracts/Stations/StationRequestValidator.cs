namespace Waslah.Contracts.Stations;

public class StationRequestValidator : AbstractValidator<StationRequest>
{
    public StationRequestValidator()
    {
        RuleFor(x => x.Government)
            .NotEmpty().WithMessage("Government is required.")
            .MaximumLength(100).WithMessage("Government must not exceed 100 characters.")
            .Matches(RegexPattern.ArabicOnlyPattern).WithMessage("Government must contain Arabic letters only.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.")
            .Matches(RegexPattern.ArabicOnlyPattern).WithMessage("City must contain Arabic letters only.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(150).WithMessage("station name must not exceed 150 characters.")
            .Matches(RegexPattern.ArabicOnlyPattern).WithMessage("Name must contain Arabic letters only.");
    }
}
