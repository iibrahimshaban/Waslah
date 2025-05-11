namespace Waslah.Contracts.Agencies;

public record AgencyRequest(
    string CompanyName,
    string PickUpCity,
    string DestinationCity,
    double Price,
    int NumberOfDays,
    string Details,
    string PhoneNumber,
    DateTime PickUpDate,
    IList<LinkDto> RelatedLinks
    );

