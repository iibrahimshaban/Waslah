namespace Waslah.Contracts.ChainedRoutes
{
    public record ChainedStationResponse(
        int LocationId,
        string? Name,
        string? City,
        string Coordinates,
        double? Distance
        );
}
