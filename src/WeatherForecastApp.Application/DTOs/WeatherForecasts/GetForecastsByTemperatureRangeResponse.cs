namespace WeatherForecastApp.Application.DTOs.WeatherForecasts;

public class GetForecastsByTemperatureRangeResponse
{
    public IEnumerable<WeatherForecastDto> Forecasts { get; set; } = Enumerable.Empty<WeatherForecastDto>();
    public int Count => Forecasts.Count();
    public int MinTemperature { get; set; }
    public int MaxTemperature { get; set; }
}
