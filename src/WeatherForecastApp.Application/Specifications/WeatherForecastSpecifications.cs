namespace WeatherForecastApp.Application.Specifications;

public class UpcomingWeekSpecification : BaseSpecification<WeatherForecast>
{
    public override Expression<Func<WeatherForecast, bool>> ToExpression()
    {
        var today = DateTime.UtcNow.Date;
        var nextWeek = today.AddDays(7);
        return forecast => forecast.Date >= today && forecast.Date <= nextWeek;
    }
}

public class TemperatureRangeSpecification(int minTemperature, int maxTemperature) : BaseSpecification<WeatherForecast>
{
    public override Expression<Func<WeatherForecast, bool>> ToExpression()
    {
        return forecast => forecast.Temperature.Celsius >= minTemperature && forecast.Temperature.Celsius <= maxTemperature;
    }
}

public class MinimumTemperatureSpecification(int minTemperature) : BaseSpecification<WeatherForecast>
{
    public override Expression<Func<WeatherForecast, bool>> ToExpression()
    {
        return forecast => forecast.Temperature.Celsius >= minTemperature;
    }
}

public class MaximumTemperatureSpecification(int maxTemperature) : BaseSpecification<WeatherForecast>
{
    public override Expression<Func<WeatherForecast, bool>> ToExpression()
    {
        return forecast => forecast.Temperature.Celsius <= maxTemperature;
    }
}

public class SummaryContainsSpecification(string searchTerm) : BaseSpecification<WeatherForecast>
{
    public override Expression<Func<WeatherForecast, bool>> ToExpression()
    {
        return forecast => forecast.Summary.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
    }
}

public class DateRangeSpecification(DateTime startDate, DateTime endDate) : BaseSpecification<WeatherForecast>
{
    public override Expression<Func<WeatherForecast, bool>> ToExpression()
    {
        return forecast => forecast.Date >= startDate && forecast.Date <= endDate;
    }
}
