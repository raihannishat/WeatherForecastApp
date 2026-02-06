namespace WeatherForecastApp.Domain.Base;

public abstract class BaseEntity : IAggregateRoot
{
    public string Id { get; protected set; } = string.Empty;
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    internal void SetId(string id)
    {
        if (string.IsNullOrEmpty(Id))
            Id = id;
    }

    internal void SetCreatedAt(DateTime createdAt)
    {
        if (CreatedAt == default)
            CreatedAt = createdAt;
    }

    public void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}

public interface IAggregateRoot
{
    string Id { get; }
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
}