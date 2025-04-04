namespace Waslah.Entities
{
    public sealed class ChainedRoute
    {
        public int Id { get; set; }
        public int FirstLocId { get; set; }
        public int LastLocId { get; set; }
        public string Name { get; set; }= string.Empty;
        public double? Distance { get; set; }
        public double? Price { get; set; }
        public double? Time { get; set; }
        public int NumberOfRides { get; set; }
        public ICollection<RouteConnector> RoutesInfo { get; set; } = [];
        public ICollection<MyRoute> Routes { get; set; } = [];
        public Station FirstLoc { get; set; } = default!;
        public Station LastLoc { get; set; } = default!;
        public ICollection<ApplicationUser> Users { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];

    }
}
