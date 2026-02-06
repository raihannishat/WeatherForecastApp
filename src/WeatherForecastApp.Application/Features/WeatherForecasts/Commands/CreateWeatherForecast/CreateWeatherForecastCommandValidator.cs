namespace WeatherForecastApp.Application.Features.WeatherForecasts.Commands.CreateWeatherForecast;

public class CreateWeatherForecastCommandValidator : AbstractValidator<CreateWeatherForecastCommand>
{
    public CreateWeatherForecastCommandValidator()
    {
        RuleFor(x => x.Dto.Date).NotEmpty();
        RuleFor(x => x.Dto.TemperatureC).InclusiveBetween(-100, 100);
        RuleFor(x => x.Dto.Summary).NotEmpty().MaximumLength(150);
    }
}