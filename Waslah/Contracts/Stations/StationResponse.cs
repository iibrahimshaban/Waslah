namespace Waslah.Contracts.Stations
{
    public record StationResponse(
      int StationId ,
     string Name ,
     string City ,
     string Government,
     bool IsActivated,
     string Coordinates ,
     string Type );
}
