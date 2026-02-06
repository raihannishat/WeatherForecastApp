namespace WeatherForecastApp.Infrastructure.Persistence.Configurations;

public class WeatherForecastConfiguration : IEntityTypeConfiguration<WeatherForecast>
{
    public void Configure(EntityTypeBuilder<WeatherForecast> builder)
    {
        // Table name
        builder.ToTable("WeatherForecasts");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties configuration
        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Date)
            .IsRequired();

        // Temperature value object mapping - use owned entity pattern
        builder.OwnsOne(x => x.Temperature, temp =>
        {
            temp.Property(t => t.Celsius)
                .HasColumnName("TemperatureC")
                .IsRequired();
            
            // Ignore Fahrenheit as it's computed
            temp.Ignore(t => t.Fahrenheit);
        });

        builder.Property(x => x.Summary)
            .IsRequired()
            .HasMaxLength(200);

        // BaseEntity properties
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        // Indexes
        builder.HasIndex(x => x.Date);
    }
}

