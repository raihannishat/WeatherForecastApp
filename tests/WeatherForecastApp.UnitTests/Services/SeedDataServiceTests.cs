namespace WeatherForecastApp.UnitTests.Services;

public class SeedDataServiceTests
{
    private readonly Mock<IWeatherForecastRepository> _repositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SeedDataService _sut;

    public SeedDataServiceTests()
    {
        _repositoryMock = new Mock<IWeatherForecastRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new SeedDataService(
            _repositoryMock.Object,
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task SeedAsync_WhenUserAlreadyExists_DoesNotAddUser()
    {
        var existingUser = User.Create("admin@example.com", "hash");
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("admin@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<WeatherForecast>());

        await _sut.SeedAsync(CancellationToken.None);

        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task SeedAsync_WhenForecastsAlreadyExist_DoesNotAddForecasts()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwordHasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed");
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<WeatherForecast> { WeatherForecast.Create(DateTime.UtcNow, Temperature.FromCelsius(0), "Cold") });

        await _sut.SeedAsync(CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<WeatherForecast>()), Times.Never);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task SeedAsync_WhenNothingExists_AddsUserAndForecasts()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwordHasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed");
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<WeatherForecast>());

        await _sut.SeedAsync(CancellationToken.None);

        _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == "admin@example.com")), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<WeatherForecast>()), Times.Exactly(30));
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }
}
