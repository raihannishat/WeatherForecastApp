namespace WeatherForecastApp.Application.Features.WeatherForecasts.Commands.CreateWeatherForecast;

public record CreateWeatherForecastCommand(WeatherForecastDto Dto) : IRequest<Result<CreateWeatherForecastResponse>>;