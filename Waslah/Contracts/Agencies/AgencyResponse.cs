namespace Waslah.Contracts.Agencies;

public record AgencyResponse(
    int Id,
    string CompanyName,
    string Road,
    double Price,
    int NumberOfDays,
    string Details,
    DateTime PickUpDate,
    string PhoneNumber,
    IList<LinkDto> RelatedLinks,
    string PhotoUrl
    );
