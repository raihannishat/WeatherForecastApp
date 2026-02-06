namespace WeatherForecastApp.Application.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWeatherForecastRepository _repository;
    private readonly IMapper _mapper;

    public WeatherForecastService(
        IUnitOfWork unitOfWork, 
        IWeatherForecastRepository repository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<WeatherForecastDto> CreateForecastAsync(WeatherForecastDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var entity = _mapper.Map<WeatherForecastDto, WeatherForecast>(dto);
            await _repository.AddAsync(entity);

            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitTransactionAsync();
            
            // Return DTO instead of entity
            return _mapper.Map<WeatherForecast, WeatherForecastDto>(entity);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw; // Let global exception handler handle it
        }
    }

    public async Task<IEnumerable<WeatherForecastDto>> GetAllForecastsAsync()
    {
        var forecasts = await _repository.GetAllAsync();
        return _mapper.Map<WeatherForecast, WeatherForecastDto>(forecasts);
    }

    public async Task<IEnumerable<WeatherForecastDto>> GetNextWeekForecastsAsync()
    {
        var forecasts = await _repository.GetUpcomingWeekForecasts();
        return _mapper.Map<WeatherForecast, WeatherForecastDto>(forecasts);
    }

    public async Task<IEnumerable<WeatherForecastDto>> GetForecastsByTemperatureRangeAsync(int minTemp, int maxTemp)
    {
        var forecasts = await _repository.GetForecastsByTemperatureRange(minTemp, maxTemp);
        return _mapper.Map<WeatherForecast, WeatherForecastDto>(forecasts);
    }
}
