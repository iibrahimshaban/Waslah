namespace Waslah.Contracts.Agencies;

public class AgencyRequestValidator : AbstractValidator<AgencyRequest>
{
    public AgencyRequestValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required.");

        RuleFor(x => x.PickUpCity)
            .NotEmpty().WithMessage("Pick-up city is required.");

        RuleFor(x => x.DestinationCity)
            .NotEmpty().WithMessage("Destination city is required.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.NumberOfDays)
            .GreaterThan(0).WithMessage("Number of days must be greater than 0.");

        RuleFor(x => x.Details)
            .NotEmpty().WithMessage("Details are required.");    

        RuleFor(x => x.PickUpDate)
            .NotEmpty().WithMessage("invalid date")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Pick-up date must be today or later.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(RegexPattern.PhoneNumber).WithMessage("Phone number format is invalid.");

        RuleFor(x => x.RelatedLinks)
            .NotEmpty().NotNull().WithMessage("Trip need at least one Link");

        RuleFor(x => x.RelatedLinks)
            .Must(x => x.Distinct().Count() == x.Count()).WithMessage("doblicated Links for the same Trip")
            .When(x => x.RelatedLinks is not null);

        RuleFor(x => x.RelatedLinks)
            .NotNull().WithMessage("Links list cannot be null.")
            .Must(links => links.Count <= 3).WithMessage("You can only send up to 3 links.");

        RuleForEach(x => x.RelatedLinks).SetValidator(new LinkDtoValidator());
    }
}
