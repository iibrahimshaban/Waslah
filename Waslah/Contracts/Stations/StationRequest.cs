namespace Waslah.Contracts.Stations
{
    public record StationRequest(
        [Required,ValidLocation] string Point
        );
}
