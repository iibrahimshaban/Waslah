namespace Waslah.Errors
{
    public static class StationErrors
    {
        public static readonly Error NotFound = new(
            "Station.StationNotFound", "there isn't a nearby station for the given coordinates", StatusCodes.Status404NotFound);
        public static readonly Error InvalidCoordinates = new(
            "Station.InvalidCoordinates", "can't evalute the points", StatusCodes.Status400BadRequest);
        public static readonly Error DoublicatedStation = new(
            "Station.DoublicatedStation", "you have inserted the same station before ", StatusCodes.Status409Conflict);
    }
}
