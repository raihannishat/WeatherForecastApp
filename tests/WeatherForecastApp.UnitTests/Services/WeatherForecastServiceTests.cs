namespace WeatherForecastApp.UnitTests.Services;

public class WeatherForecastServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IWeatherForecastRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly WeatherForecastService _sut;

    public WeatherForecastServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IWeatherForecastRepository>();
        _mapperMock = new Mock<IMapper>();
        _sut = new WeatherForecastService(_unitOfWorkMock.Object, _repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateForecastAsync_CallsUnitOfWorkAndRepository_ReturnsMappedDto()
    {
        var dto = new WeatherForecastDto
        {
            Date = DateTime.UtcNow,
            TemperatureC = 25,
            TemperatureF = 77,
            Summary = "Warm"
        };
        var entity = WeatherForecast.Create(DateTime.UtcNow, Temperature.FromCelsius(25), "Warm");
        _mapperMock.Setup(m => m.Map<WeatherForecastDto, WeatherForecast>(dto)).Returns(entity);
        _mapperMock.Setup(m => m.Map<WeatherForecast, WeatherForecastDto>(entity)).Returns(dto);

        var result = await _sut.CreateForecastAsync(dto);

        Assert.NotNull(result);
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<WeatherForecast>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllForecastsAsync_ReturnsMappedDtos()
    {
        var forecasts = new List<WeatherForecast>
        {
            WeatherForecast.Create(DateTime.UtcNow, Temperature.FromCelsius(10), "Cool")
        };
        var dtos = new List<WeatherForecastDto> { new() { Summary = "Cool" } };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(forecasts);
        _mapperMock.Setup(m => m.Map<WeatherForecast, WeatherForecastDto>(It.IsAny<IEnumerable<WeatherForecast>>())).Returns(dtos);

        var result = await _sut.GetAllForecastsAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetNextWeekForecastsAsync_ReturnsMappedDtos()
    {
        var forecasts = new List<WeatherForecast>();
        _repositoryMock.Setup(r => r.GetUpcomingWeekForecasts()).ReturnsAsync(forecasts);
        _mapperMock.Setup(m => m.Map<WeatherForecast, WeatherForecastDto>(It.IsAny<IEnumerable<WeatherForecast>>()))
            .Returns(new List<WeatherForecastDto> { new() { Summary = "Cool" } });

        var result = await _sut.GetNextWeekForecastsAsync();

        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.GetUpcomingWeekForecasts(), Times.Once);
    }

    [Fact]
    public async Task GetForecastsByTemperatureRangeAsync_ReturnsMappedDtos()
    {
        var forecasts = new List<WeatherForecast>();
        _repositoryMock.Setup(r => r.GetForecastsByTemperatureRange(5, 15)).ReturnsAsync(forecasts);
        _mapperMock.Setup(m => m.Map<WeatherForecast, WeatherForecastDto>(It.IsAny<IEnumerable<WeatherForecast>>()))
            .Returns(new List<WeatherForecastDto>());

        var result = await _sut.GetForecastsByTemperatureRangeAsync(5, 15);

        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.GetForecastsByTemperatureRange(5, 15), Times.Once);
    }
}
