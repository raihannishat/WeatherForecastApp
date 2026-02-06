namespace WeatherForecastApp.Application.Features.WeatherForecasts.Queries.GetForecastsByTemperatureRange;

public record GetForecastsByTemperatureRangeQuery(int MinTemperature, int MaxTemperature) : IRequest<Result<GetForecastsByTemperatureRangeResponse>>;

