namespace WeatherForecastApp.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IAuthService authService)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        => authService.LoginAsync(request.Request, cancellationToken);
}
