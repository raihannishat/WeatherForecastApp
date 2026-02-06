namespace WeatherForecastApp.Application.Mappings;

public class WeatherForecastMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Entity -> DTO mapping
        config.NewConfig<WeatherForecast, WeatherForecastDto>()
            .Map(dest => dest.TemperatureC, src => src.Temperature.Celsius)
            .Map(dest => dest.TemperatureF, src => src.Temperature.Fahrenheit);
        
        // DTO -> Entity mapping
        config.NewConfig<WeatherForecastDto, WeatherForecast>()
            .MapWith(src => WeatherForecast.Create(
                src.Date,
                Domain.ValueObjects.Temperature.FromCelsius(src.TemperatureC),
                src.Summary));
        
        // DTO <-> Response mappings
        config.NewConfig<WeatherForecastDto, CreateWeatherForecastResponse>()
            .Ignore(dest => dest.Message); // Message will be set manually
        
        // Collection to Response mapping
        config.NewConfig<IEnumerable<WeatherForecastDto>, GetAllWeatherForecastsResponse>()
            .Map(dest => dest.Forecasts, src => src)
            .Map(dest => dest.Count, src => src.Count());
        
        // Temperature Range Response mapping
        config.NewConfig<IEnumerable<WeatherForecastDto>, GetForecastsByTemperatureRangeResponse>()
            .Map(dest => dest.Forecasts, src => src)
            .Map(dest => dest.Count, src => src.Count());
    }
}

