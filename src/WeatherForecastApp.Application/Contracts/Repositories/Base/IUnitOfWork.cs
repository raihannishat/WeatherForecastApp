namespace WeatherForecastApp.Application.Contracts.Repositories.Base;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
