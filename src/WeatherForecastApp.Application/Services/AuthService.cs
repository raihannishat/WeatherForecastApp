namespace WeatherForecastApp.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IUnitOfWork unitOfWork,
    IOptions<JwtSettings> jwtSettings) : IAuthService
{
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            return Result<LoginResponse>.Failure("Invalid email or password.");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<LoginResponse>.Failure("Invalid email or password.");

        var token = jwtTokenService.GenerateToken(user.Id, user.Email);
        var expiresAt = DateTime.UtcNow.AddMinutes(jwtSettings.Value.ExpirationMinutes);
        return Result<LoginResponse>.Success(new LoginResponse(token, "Bearer", expiresAt));
    }

    public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            return Result<RegisterResponse>.Failure("A user with this email already exists.");

        await unitOfWork.BeginTransactionAsync();
        try
        {
            var passwordHash = passwordHasher.Hash(request.Password);
            var user = User.Create(request.Email, passwordHash);
            await userRepository.AddAsync(user);

            await unitOfWork.SaveAsync();
            await unitOfWork.CommitTransactionAsync();

            return Result<RegisterResponse>.Success(
                new RegisterResponse(user.Id, user.Email, "User registered successfully."));
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
