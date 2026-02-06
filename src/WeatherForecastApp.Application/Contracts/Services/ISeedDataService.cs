namespace WeatherForecastApp.Application.Contracts.Services;

public interface ISeedDataService
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
