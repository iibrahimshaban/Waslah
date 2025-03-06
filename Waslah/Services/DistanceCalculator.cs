namespace Waslah.Services
{
    public class DistanceCalculator : IDistanceCalculator
    {
        private const double EarthRadiusKm = 6371;
        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double lat1Rad = DegreesToRadians(lat1);
            double lon1Rad = DegreesToRadians(lon1);
            double lat2Rad = DegreesToRadians(lat2);
            double lon2Rad = DegreesToRadians(lon2);

            // Difference in coordinates
            double dLat = lat2Rad - lat1Rad;
            double dLon = lon2Rad - lon1Rad;

            // Haversine formula
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Distance in kilometers
            double distance = EarthRadiusKm * c;
            return distance;
        }

        public double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
