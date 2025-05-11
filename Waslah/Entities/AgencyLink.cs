namespace Waslah.Entities;

public class AgencyLink
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int AgencyId { get; set; }
    public AgencyTrip AgencyTrip { get; set; } = default!;

}
