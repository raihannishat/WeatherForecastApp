namespace WeatherForecastApp.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        string connectionString)
    {
        // Database Configuration
        services.AddDbContext<WeatherForecastDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repository Registration
        services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Auth Services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        // Seed (uses UoW + repository only)
        services.AddScoped<ISeedDataService, SeedDataService>();

        return services;
    }
}

