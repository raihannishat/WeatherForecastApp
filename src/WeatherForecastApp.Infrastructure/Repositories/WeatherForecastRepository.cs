namespace WeatherForecastApp.Infrastructure.Repositories;

public class WeatherForecastRepository(WeatherForecastDbContext context) 
    : GenericRepository<WeatherForecast>(context), IWeatherForecastRepository
{
    public async Task<IEnumerable<WeatherForecast>> GetUpcomingWeekForecasts()
    {
        var specification = new UpcomingWeekSpecification();
        return await FindBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<WeatherForecast>> GetForecastsByTemperature(int temp)
    {
        var specification = new MinimumTemperatureSpecification(temp);
        return await FindBySpecificationAsync(specification);
    }

    public async Task<IEnumerable<WeatherForecast>> GetForecastsByTemperatureRange(int minTemp, int maxTemp)
    {
        var specification = new TemperatureRangeSpecification(minTemp, maxTemp);
        return await FindBySpecificationAsync(specification);
    }
}