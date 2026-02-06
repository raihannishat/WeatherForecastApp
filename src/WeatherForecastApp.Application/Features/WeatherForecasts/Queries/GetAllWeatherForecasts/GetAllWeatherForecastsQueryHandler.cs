namespace WeatherForecastApp.Application.Features.WeatherForecasts.Queries.GetAllWeatherForecasts;

public class GetAllWeatherForecastsQueryHandler(
    IWeatherForecastService service,
    IMapper mapper) 
    : IRequestHandler<GetAllWeatherForecastsQuery, Result<GetAllWeatherForecastsResponse>>
{
    public async Task<Result<GetAllWeatherForecastsResponse>> Handle(GetAllWeatherForecastsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var forecastDtos = await service.GetAllForecastsAsync();
            
            // Use object mapper instead of manual object creation
            var response = mapper.Map<IEnumerable<WeatherForecastDto>, GetAllWeatherForecastsResponse>(forecastDtos);

            return Result<GetAllWeatherForecastsResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetAllWeatherForecastsResponse>.Failure(ex.Message);
        }
    }
}
