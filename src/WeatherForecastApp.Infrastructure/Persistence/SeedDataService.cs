namespace WeatherForecastApp.Infrastructure.Persistence;

public class SeedDataService(
    IWeatherForecastRepository repository,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : ISeedDataService
{
    private const string DefaultUserEmail = "admin@example.com";
    private const string DefaultUserPassword = "Admin@123";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await unitOfWork.BeginTransactionAsync();
        try
        {
            await SeedUsersAsync(cancellationToken);
            await SeedWeatherForecastsAsync(cancellationToken);

            await unitOfWork.SaveAsync();
            await unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private async Task SeedUsersAsync(CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByEmailAsync(DefaultUserEmail, cancellationToken);
        if (existingUser is not null)
            return;

        var user = User.Create(DefaultUserEmail, passwordHasher.Hash(DefaultUserPassword));
        await userRepository.AddAsync(user);
    }

    private async Task SeedWeatherForecastsAsync(CancellationToken cancellationToken)
    {
        var existing = await repository.GetAllAsync();
        if (existing.Any())
            return;

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var random = new Random();
        for (int i = 0; i < 30; i++)
        {
            var date = DateTime.UtcNow.AddDays(i);
            var temperatureC = random.Next(-20, 55);
            var summary = summaries[random.Next(summaries.Length)];
            var forecast = WeatherForecast.Create(
                date,
                Temperature.FromCelsius(temperatureC),
                summary);
            await repository.AddAsync(forecast);
        }
    }
}
