namespace Waslah.Contracts.Stations;

public record StationRequest(
    string Government,
    string City ,
    string Name ,
    [ValidLocation]string Coordinates ,
    bool IsStation
    );
