namespace Waslah.Entities
{
    public class Report
    {
        public int Id { get; set; }
        public string Type { get; set; } =string.Empty;
        public string Sevirity { get; set; } = string.Empty;
        public string UserFeedBack { get; set; } = string.Empty ;
        public string UserId { get; set; } =  string.Empty;
        public int RouteId { get; set; }
        public ApplicationUser User { get; set; } = default!;
        public MyRoute Route { get; set; } = default!;
    }
}
