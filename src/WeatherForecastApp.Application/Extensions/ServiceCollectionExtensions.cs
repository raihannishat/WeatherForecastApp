namespace WeatherForecastApp.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR Configuration
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation Configuration
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.ValidationBehavior<,>));

        // Mapster Configuration
        TypeAdapterConfig.GlobalSettings.Scan(assembly);
        services.AddSingleton(TypeAdapterConfig.GlobalSettings);
        
        // Mapping Service (Abstracted)
        services.AddScoped<Common.Mapping.IMapper, Common.Mapping.MapsterMapper>();

        // Application Services
        services.AddScoped<IWeatherForecastService, WeatherForecastService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}

