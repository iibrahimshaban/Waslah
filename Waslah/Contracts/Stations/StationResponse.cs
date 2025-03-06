namespace Waslah.Contracts.Stations
{
    public record StationResponse(
      int LocationId ,
     string? Name ,
     string? City ,
     string? Government,
     bool IsActivated,
     string Coordinates ,
     string Type );
}
