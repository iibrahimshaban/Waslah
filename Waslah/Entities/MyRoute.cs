namespace Waslah.Entities
{
    public sealed class MyRoute
    {
        public int Id { get; set; }
        public int PrimaryLocId { get; set; }
        public int SecondaryLocId { get; set; }
        public double? Distance { get; set; }
        public double? Time { get; set; }
        public double? Price { get; set; }
        public string? Classification { get; set; }= string.Empty;
        public Station PriStation { get; set; } = default!;
        public Station SecStation { get; set; } = default!;
        public ICollection<RouteConnector> connectors { get; set; } = [];
        public ICollection<ChainedRoute> chainedRoutes { get; set; } = [];

    }
}
