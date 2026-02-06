namespace WeatherForecastApp.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _jwtSettings = Options.Create(new JwtSettings
        {
            Secret = "TestSecretKeyThatIsAtLeast32CharactersLong!",
            Issuer = "Test",
            Audience = "Test",
            ExpirationMinutes = 60
        });
        _sut = new AuthService(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object,
            _unitOfWorkMock.Object,
            _jwtSettings);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ReturnsFailure()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _sut.LoginAsync(new LoginRequest("a@b.com", "pass"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid email or password", result.Errors.First());
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordInvalid_ReturnsFailure()
    {
        var user = User.Create("a@b.com", "hash");
        DomainTestHelper.SetId(user, "user-1");
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("a@b.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify("wrong", "hash")).Returns(false);

        var result = await _sut.LoginAsync(new LoginRequest("a@b.com", "wrong"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid email or password", result.Errors.First());
    }

    [Fact]
    public async Task LoginAsync_WhenValid_ReturnsSuccessWithToken()
    {
        var user = User.Create("a@b.com", "hash");
        DomainTestHelper.SetId(user, "user-1");
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("a@b.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify("pass", "hash")).Returns(true);
        _jwtTokenServiceMock.Setup(j => j.GenerateToken("user-1", "a@b.com")).Returns("jwt-token");

        var result = await _sut.LoginAsync(new LoginRequest("a@b.com", "pass"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("jwt-token", result.Value!.Token);
        Assert.Equal("Bearer", result.Value.TokenType);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailExists_ReturnsFailure()
    {
        var existing = User.Create("a@b.com", "hash");
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("a@b.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _sut.RegisterAsync(new RegisterRequest("a@b.com", "newpass"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Errors.First());
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenValid_CallsUnitOfWorkAndReturnsSuccess()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("new@b.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwordHasherMock.Setup(h => h.Hash("pass")).Returns("hashed");

        var result = await _sut.RegisterAsync(new RegisterRequest("new@b.com", "pass"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("new@b.com", result.Value!.Email);
        Assert.NotNull(result.Value.Id);
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == "new@b.com" && u.PasswordHash == "hashed")), Times.Once);
    }
}
