namespace WeatherForecastApp.Infrastructure.Repositories.Implementations;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly WeatherForecastDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(WeatherForecastDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        if (string.IsNullOrEmpty(entity.Id))
            entity.SetId(Guid.NewGuid().ToString());
        
        if (entity.CreatedAt == default)
            entity.SetCreatedAt(DateTime.UtcNow);
        
        await _dbSet.AddAsync(entity);
    }

    public void Delete(T entity) => _dbSet.Remove(entity);
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();
    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public async Task<T?> GetByIdAsync(string id) => await _dbSet.FindAsync(id);
    public async Task<IEnumerable<T>> FindBySpecificationAsync(ISpecification<T> specification)
        => await _dbSet.Where(specification.ToExpression()).ToListAsync();

    public void Update(T entity) 
    { 
        _dbSet.Update(entity); 
        entity.SetUpdatedAt(); 
    }
}
