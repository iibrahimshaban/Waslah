namespace Waslah.Entities;

public sealed class AgencyTrip
{
    public int Id { get; set; }
    public string AgencyName { get; set; } = string.Empty;
    public string PickUpCity { get; set; } = string.Empty ;
    public string DestinationCity {  get; set; } = string.Empty ;
    public double Price { get; set; }
    public double NumberOfDays { get; set; }
    public string Details { get; set; } =string.Empty ;

    public DateTime PickUpDate { get; set; }
    public string PhoneNumber {  get; set; } = string.Empty ;
    public ICollection<AgencyLink> Links { get; set; } = [];
    public string? PhotoPath {  get; set; } = string.Empty ;
    public bool IsDiasabled { get; set; } = false ;
}

