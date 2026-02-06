namespace WeatherForecastApp.Application.Features.WeatherForecasts.Queries.GetForecastsByTemperatureRange;

public class GetForecastsByTemperatureRangeQueryValidator : AbstractValidator<GetForecastsByTemperatureRangeQuery>
{
    public GetForecastsByTemperatureRangeQueryValidator()
    {
        RuleFor(x => x.MinTemperature)
            .InclusiveBetween(-100, 100)
            .WithMessage("Minimum temperature must be between -100 and 100 degrees Celsius");

        RuleFor(x => x.MaxTemperature)
            .InclusiveBetween(-100, 100)
            .WithMessage("Maximum temperature must be between -100 and 100 degrees Celsius");

        RuleFor(x => x.MaxTemperature)
            .GreaterThanOrEqualTo(x => x.MinTemperature)
            .WithMessage("Maximum temperature must be greater than or equal to minimum temperature");
    }
}

