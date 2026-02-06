namespace WeatherForecastApp.Application.Contracts.Services.Auth;

public interface IJwtTokenService
{
    string GenerateToken(string userId, string email);
}
