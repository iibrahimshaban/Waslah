namespace Waslah.Errors
{
    public static class OrderErrors
    {
        public static readonly Error RouteNotFound = new(
            "Order.RouteNotFound", "can't find route with given information", StatusCodes.Status404NotFound);

        public static readonly Error UserNotFound = new(
            "Order.UserNotFound", "Invalid User", StatusCodes.Status404NotFound);

        public static readonly Error TripsNotFound = new(
            "Order.TripsNotFound", "there is not any trips for this user yet", StatusCodes.Status404NotFound);
    }
}
