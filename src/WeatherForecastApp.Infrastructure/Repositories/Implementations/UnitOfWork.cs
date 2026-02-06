namespace WeatherForecastApp.Infrastructure.Repositories.Implementations;

public class UnitOfWork(WeatherForecastDbContext context) : IUnitOfWork
{
    private readonly WeatherForecastDbContext _context = context;
    private IDbContextTransaction? _transaction;

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        // Only dispose transaction, not DbContext
        // DbContext is managed by DI container
        _transaction?.Dispose();
    }
}