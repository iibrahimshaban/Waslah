namespace Waslah.Contracts.Stations
{
    public record StationDistanceResponse(
        int LocationId,
     string? Name,
     string? City,
     string coordinates,
     string Type,
     double Distance);
}
