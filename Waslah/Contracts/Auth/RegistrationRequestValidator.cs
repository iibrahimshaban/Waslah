using Waslah.Abstraction.Consts;

namespace Waslah.Contracts.Auth;

public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
{
    public RegistrationRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("email is required")
            .EmailAddress().WithMessage("invalid email address");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(RegexPattern.PhoneNumber).WithMessage("Invalid phone number format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("can't accept an empty password")
            .Matches(RegexPattern.Password).WithMessage("invalid password");

        RuleFor(x => x.FirstName).NotEmpty().WithMessage("first name is required");

        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required");
    }
}
