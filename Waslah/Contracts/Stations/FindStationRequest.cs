namespace Waslah.Contracts.Stations
{
    public record FindStationRequest(
        [Required,ValidLocation] string Point
        );
}
