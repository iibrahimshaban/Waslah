namespace Waslah.Contracts.Agencies;

public class LinkDtoValidator : AbstractValidator<LinkDto>
{
    public LinkDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Link name is required.");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Link URL is required.")
            .Matches(RegexPattern.UrlPattern).WithMessage("Link URL must be a valid URL.");
    }
}
