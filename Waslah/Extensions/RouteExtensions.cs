using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Waslah.Extensions
{
    public static class RouteExtensions
    {
        public static RoutePointsRequest EvaluatePoints(this FindRouteRequest RouteRequest)
        {
            string[] Sparts = RouteRequest.Start.Split(',');
            string[] Eparts = RouteRequest.End.Split(',');

            var Points = new RoutePointsRequest(
                StartLatitude : double.Parse(Sparts[0]),
                StartLongitude : double.Parse(Sparts[1]),
                EndtLatitude : double.Parse(Eparts[0]),
                EndLongitude : double.Parse(Eparts[1])
            );

            return Points;
        }
        
    }
}
