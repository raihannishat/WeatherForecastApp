namespace WeatherForecastApp.Application.DTOs.Auth;

public record LoginResponse(string Token, string TokenType, DateTime ExpiresAt);
