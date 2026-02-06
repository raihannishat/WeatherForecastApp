namespace WeatherForecastApp.Application.DTOs.WeatherForecasts;

public class GetAllWeatherForecastsResponse
{
    public IEnumerable<WeatherForecastDto> Forecasts { get; set; } = [];
    public int Count { get; set; }
}
