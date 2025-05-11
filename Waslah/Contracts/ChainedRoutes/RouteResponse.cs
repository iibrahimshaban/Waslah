namespace Waslah.Contracts.ChainedRoutes;

public record RouteResponse(
    int Id,
    string FirstStation,
    string FirstCity,
    string LastStation,
    string LastCity,
    int NumberOfRides
    );
