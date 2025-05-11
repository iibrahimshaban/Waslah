namespace Waslah.Errors
{
    public static class RouteErrors
    {
        public static readonly Error NotFound = new(
            "Route.NotFound", "can't find route with given information", StatusCodes.Status404NotFound);
        public static readonly Error CityMatched = new(
            "Route.CityDonotMatch", "the station to city method is not working", StatusCodes.Status404NotFound);
        public static readonly Error Doublicated = new(
            "Route.Doublicated", "can't save this route as it's already saved", StatusCodes.Status409Conflict);
        public static readonly Error NotGenerated = new(
            "Route.NotGeneratedYet", "can't generate route ", StatusCodes.Status409Conflict);
    }
}
