namespace WeatherForecastApp.Application.Contracts.Repositories;

public interface IWeatherForecastRepository : IGenericRepository<WeatherForecast>
{
    Task<IEnumerable<WeatherForecast>> GetUpcomingWeekForecasts();
    Task<IEnumerable<WeatherForecast>> GetForecastsByTemperature(int temp);
    Task<IEnumerable<WeatherForecast>> GetForecastsByTemperatureRange(int minTemp, int maxTemp);
}
