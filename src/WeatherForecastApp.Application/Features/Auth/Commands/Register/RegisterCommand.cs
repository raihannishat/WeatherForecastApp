namespace WeatherForecastApp.Application.Features.Auth.Commands.Register;

public record RegisterCommand(RegisterRequest Request) : IRequest<Result<RegisterResponse>>;
