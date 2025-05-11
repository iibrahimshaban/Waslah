namespace Waslah.Contracts.Agencies;

public class AgencyPhotoRequestValidator : AbstractValidator<AgencyPhotoRequest>
{
    public AgencyPhotoRequestValidator()
    {
        RuleFor(x => x.Photo)
           .Must(BeAValidImage).WithMessage("Only JPG , JPEG and PNG images are allowed.")
           .Must(f => f == null || f.Length <= 2 * 1024 * 1024).WithMessage("Image size must be 2MB or less.");

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
