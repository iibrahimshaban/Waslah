namespace Waslah.Entities
{
    public sealed class UserPoints
    {
        public int Id { get; set; } 
        public double OriginLatitude { get; set; }
        public double OriginLongitude { get; set; }
        public double DestinationLatitude { get; set; }
        public double DestinationLongitude { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public bool IsLocked { get; set; } = true;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = default!;
    }
}
