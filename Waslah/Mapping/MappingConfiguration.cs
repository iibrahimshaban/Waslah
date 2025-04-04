namespace Waslah.Mapping
{
    public class MappingConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {

            config.NewConfig<StationDistances, StationDistanceResponse>()
                .Map(dest => dest.LocationId , src => src.Station.LocationId)
                .Map(dest => dest.Distance, src => src.Distance)
                .Map(dest => dest.Name, src => src.Station.Name)
                .Map(dest => dest.coordinates, src => src.Station.Latitude + ", " + src.Station.Longitude)
                .Map(dest => dest.City, src => src.Station.City)
                .Map(dest => dest.Type ,src => src.Station.StationType.Description);
           
            config.NewConfig<Station, StationResponse>()
                .Map(dest => dest.Coordinates, src => src.Latitude + ", " + src.Longitude);

            config.NewConfig<MyRoute, ListedRouteResponse>()
                .Map(dest => dest.StartName, src => src.PriStation.Name)
                .Map(dest => dest.StartCoordinates, src => src.PriStation.Latitude + " ," + src.PriStation.Longitude)
                .Map(dest => dest.EndName, src => src.SecStation.Name)
                .Map(dest => dest.EndCoordinates, src => src.SecStation.Latitude + " ," + src.SecStation.Longitude);

            config.NewConfig<Order, OrderResponse>()
                .Map(dest => dest.ChainedRouteId , src => src.Route.Id)
                .Map(dest => dest.StartStation, src => src.Route.FirstLoc.Name)
                .Map(dest => dest.DestinationStation, src => src.Route.LastLoc.Name)
                .Map(dest => dest.UserName, src => src.User.FirstName + ' ' + src.User.LastName)
                .Map(dest => dest.UsedAt, src => DateOnly.FromDateTime(src.UsedAt));
                

        }
    }
}
