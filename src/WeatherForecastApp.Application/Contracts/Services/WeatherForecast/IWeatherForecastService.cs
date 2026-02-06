namespace WeatherForecastApp.Application.Contracts.Services.WeatherForecast;

public interface IWeatherForecastService
{
    Task<WeatherForecastDto> CreateForecastAsync(WeatherForecastDto dto);
    Task<IEnumerable<WeatherForecastDto>> GetAllForecastsAsync();
    Task<IEnumerable<WeatherForecastDto>> GetNextWeekForecastsAsync();
    Task<IEnumerable<WeatherForecastDto>> GetForecastsByTemperatureRangeAsync(int minTemp, int maxTemp);
}
