using System.ComponentModel.DataAnnotations;

namespace Waslah.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class ValidLocationAttribute : ValidationAttribute
    {
        private const double EgyptMinLatitude = 22.0;
        private const double EgyptMaxLatitude = 31.5;
        private const double EgyptMinLongitude = 25.0;
        private const double EgyptMaxLongitude = 35.0;
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            if (value is not null)
            {
                var Coordinates = (string)value;
                string[] parts = Coordinates.Split(',');

                if (parts.Length != 2)
                    return new ValidationResult(
                        "Invalid input. Expected format: 'latitude,longitude'.");

                if (!double.TryParse(parts[0], out double latitude) || !double.TryParse(parts[1], out double longitude))
                    return new ValidationResult(
                        "Invalid coordinate format. Could not parse to double.");

                if (latitude < EgyptMinLatitude || latitude > EgyptMaxLatitude ||
                    longitude < EgyptMinLongitude || longitude > EgyptMaxLongitude)

                    return new ValidationResult(
                        "Coordinates are outside Egypt's geographic bounds.");
            }
            return ValidationResult.Success;
        }
    }
}
