namespace Waslah.Contracts.Stations;

public record CityResponse(
    string City,
    IEnumerable<StationDto> Stations
    );

