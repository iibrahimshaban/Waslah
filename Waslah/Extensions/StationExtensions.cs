namespace Waslah.Extensions
{
    public static class StationExtensions
    {
        public static int ToCity(this int LocationId)
        {
            int CityNo = LocationId / 100;
            return CityNo;
        }
        public static int ToGovernment(this int LocationId)
        {
            int GovNo = LocationId / 10000;
            return GovNo * 10000;
        }
    }
}
