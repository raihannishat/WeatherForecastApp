namespace WeatherForecastApp.Application.DTOs.WeatherForecasts;

public class WeatherForecastDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF { get; set; }
    public string Summary { get; set; } = string.Empty;
}
