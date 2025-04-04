namespace Waslah.Errors
{
    public static class OrderErrors
    {
        public static readonly Error NotFound = new(
            "Route.NotFound", "can't find route with given information", StatusCodes.Status404NotFound);
    }
}
