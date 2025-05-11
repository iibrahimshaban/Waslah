namespace Waslah.Errors;

public static class AgencyTripErrors
{
    public static readonly Error TripNotFound = new(
      "Agency.TripNotFound", "trip not found ", StatusCodes.Status404NotFound);
    public static readonly Error CompanyNotFound = new(
      "Agency.CompanyNotFound", "Company not found ", StatusCodes.Status404NotFound);

    public static readonly Error DoublicatedTrip = new(
        "Agency.DoublicatedTrip", "trip doublication ", StatusCodes.Status409Conflict);
}
