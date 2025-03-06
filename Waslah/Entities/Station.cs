namespace Waslah.Entities
{
    public sealed class Station
    {
        public int Id {  get; set; }
        public int LocationId { get; set; }
        public string? ModelId { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? Government { get; set; } = string.Empty;
        public bool? IsActivated { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int? CityNo { get; set; }
        public int? StationNo { get; set; }
        public int? GovNo { get; set; }
        public string? Type {  get; set; } = string.Empty;
        public StationType StationType { get; set; } = default!;
        public ICollection<MyRoute> GoingRoute { get; set; } = [];
        public ICollection<MyRoute> ReturnedRoute { get; set; } = [] ;
        public ICollection<ChainedRoute> StartChainedRoutes { get; set; } = [] ;
        public ICollection<ChainedRoute> EndChainedRoutes { get; set; } = [] ;
    }
}
