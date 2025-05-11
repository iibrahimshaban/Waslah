namespace Waslah.Contracts.Users;
public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.UserName)
           .NotEmpty().WithMessage("Username is required.")
           .Matches(RegexPattern.UserName).WithMessage("Username can only have letters and numbers");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.ProfilePhoto)
            .Must(BeAValidImage)
            .WithMessage("Only JPG , JPEG and PNG images are allowed.")
            .When(f => f.ProfilePhoto != null)
            .Must(f => f == null || f.Length <= 2 * 1024 * 1024)
            .WithMessage("Image size must be 2MB or less.")
            .When(f => f.ProfilePhoto != null);
    }

    private bool BeAValidImage(IFormFile? file)
    {
        if (file == null) return true; // Allow null

        var allowedContentTypes = new[] { "image/jpeg", "image/png" };
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

        var contentTypeIsValid = allowedContentTypes.Contains(file.ContentType);
        var extensionIsValid = allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant());

        return contentTypeIsValid && extensionIsValid;
    }
}

