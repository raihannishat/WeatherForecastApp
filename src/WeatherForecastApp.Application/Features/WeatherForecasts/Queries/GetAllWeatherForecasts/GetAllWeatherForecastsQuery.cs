namespace WeatherForecastApp.Application.Features.WeatherForecasts.Queries.GetAllWeatherForecasts;

public record GetAllWeatherForecastsQuery() : IRequest<Result<GetAllWeatherForecastsResponse>>;
