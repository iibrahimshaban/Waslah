namespace Waslah.Services
{
    public interface IDistanceCalculator
    {
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
        double DegreesToRadians(double degrees);
    }
}
