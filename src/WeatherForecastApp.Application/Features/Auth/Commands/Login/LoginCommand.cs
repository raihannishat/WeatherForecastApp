namespace WeatherForecastApp.Application.Features.Auth.Commands.Login;

public record LoginCommand(LoginRequest Request) : IRequest<Result<LoginResponse>>;
