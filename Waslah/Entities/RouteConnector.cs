namespace Waslah.Entities
{
    public sealed class RouteConnector
    {
        public int MyRouteId { get; set; }
        public int ChainedRouteID { get; set; }
        public int RouteOrder { get; set; }
        public MyRoute MyRoute { get; set; } = default!;
        public ChainedRoute ChainedRoute { get; set; } = default!;

    }
}
