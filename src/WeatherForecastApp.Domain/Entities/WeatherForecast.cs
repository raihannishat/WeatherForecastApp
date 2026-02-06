namespace WeatherForecastApp.Domain.Entities;

public class WeatherForecast : BaseEntity
{
    public DateTime Date { get; private set; }
    public Temperature Temperature { get; private set; } = null!;
    public string Summary { get; private set; } = string.Empty;

    // Factory method for creating new WeatherForecast
    public static WeatherForecast Create(DateTime date, Temperature temperature, string summary)
    {
        return new WeatherForecast
        {
            Date = date,
            Temperature = temperature,
            Summary = summary
        };
    }

    // Methods for updating
    public void UpdateDate(DateTime date)
    {
        Date = date;
        SetUpdatedAt();
    }

    public void UpdateTemperature(Temperature temperature)
    {
        Temperature = temperature;
        SetUpdatedAt();
    }

    public void UpdateSummary(string summary)
    {
        Summary = summary;
        SetUpdatedAt();
    }

    // For EF Core - private parameterless constructor
    private WeatherForecast() { }
}