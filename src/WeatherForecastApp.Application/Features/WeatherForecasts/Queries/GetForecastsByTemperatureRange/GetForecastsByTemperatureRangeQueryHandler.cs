namespace WeatherForecastApp.Application.Features.WeatherForecasts.Queries.GetForecastsByTemperatureRange;

public class GetForecastsByTemperatureRangeQueryHandler(
    IWeatherForecastService service,
    IMapper mapper) 
    : IRequestHandler<GetForecastsByTemperatureRangeQuery, Result<GetForecastsByTemperatureRangeResponse>>
{
    public async Task<Result<GetForecastsByTemperatureRangeResponse>> Handle(
        GetForecastsByTemperatureRangeQuery request, 
        CancellationToken cancellationToken)
    {
        var forecastDtos = await service.GetForecastsByTemperatureRangeAsync(
            request.MinTemperature, 
            request.MaxTemperature);

        var response = mapper.Map<IEnumerable<WeatherForecastDto>, GetForecastsByTemperatureRangeResponse>(forecastDtos);
        response.MinTemperature = request.MinTemperature;
        response.MaxTemperature = request.MaxTemperature;

        return Result<GetForecastsByTemperatureRangeResponse>.Success(response);
    }
}

