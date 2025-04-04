namespace Waslah.Entities
{
    public sealed class Order
    {
        public int Id { get; set; }
        public DateTime UsedAt { get; set; }= DateTime.UtcNow;

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int RouteId { get; set; }
        public double DistanceFromStart { get; set; }
        public double DistanceFromDestination { get; set; }
        public ApplicationUser User { get; set; } = default!;
        public ChainedRoute Route { get; set; } = default!;
    }
}
