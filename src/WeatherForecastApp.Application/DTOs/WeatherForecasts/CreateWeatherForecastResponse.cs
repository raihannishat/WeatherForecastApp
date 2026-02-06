namespace WeatherForecastApp.Application.DTOs.WeatherForecasts;

public class CreateWeatherForecastResponse
{
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Message { get; set; } = "Weather forecast created successfully";
}
