namespace Waslah.Contracts.Stations;

public record GovernmentResponse(
    string Government,
    IEnumerable<CityResponse> Cities
    );

