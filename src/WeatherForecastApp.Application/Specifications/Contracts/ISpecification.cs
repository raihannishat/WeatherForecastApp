namespace WeatherForecastApp.Application.Specifications.Contracts;

public interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);
    Expression<Func<T, bool>> ToExpression();
}

