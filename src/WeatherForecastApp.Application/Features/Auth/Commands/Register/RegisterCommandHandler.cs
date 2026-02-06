namespace WeatherForecastApp.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(IAuthService authService)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        => authService.RegisterAsync(request.Request, cancellationToken);
}
