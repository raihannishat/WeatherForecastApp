namespace WeatherForecastApp.Application.Common.Mapping;

public class MapsterMapper : IMapper
{
    public TDestination Map<TDestination>(object source)
    {
        return source.Adapt<TDestination>();
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return source.Adapt<TDestination>();
    }

    public IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> source)
    {
        return source.Adapt<IEnumerable<TDestination>>();
    }
}

