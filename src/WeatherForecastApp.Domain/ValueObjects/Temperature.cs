namespace WeatherForecastApp.Domain.ValueObjects;

public record Temperature
{
    public int Celsius { get; init; }
    public int Fahrenheit => 32 + (int)(Celsius / 0.5556);

    public Temperature(int celsius)
    {
        if (celsius < -100 || celsius > 100)
            throw new DomainException("Temperature must be between -100 and 100 degrees Celsius");
        
        Celsius = celsius;
    }

    public static Temperature FromCelsius(int celsius) => new(celsius);
    
    public static Temperature FromFahrenheit(int fahrenheit) => new((int)((fahrenheit - 32) * 0.5556));
}

