namespace Waslah.Errors
{
    public static class StationErrors
    {
        public static readonly Error NotFound = new("Station.NotFound"
           , "there isn't a nearby station for the given coordinates", StatusCodes.Status404NotFound);
    }
}
