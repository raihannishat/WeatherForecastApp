namespace WeatherForecastApp.Application.Common;

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public IReadOnlyList<string> Errors { get; }

    private Result(T? value, bool isSuccess, IReadOnlyList<string>? errors)
    {
        Value = value;
        IsSuccess = isSuccess;
        Errors = errors ?? Array.Empty<string>();
        
        if (isSuccess && value is null && default(T) is not null)
        {
            throw new ArgumentNullException(nameof(value), "Value cannot be null for a successful result.");
        }
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, true, null);
    }

    public static Result<T> Failure(string error, List<string>? errors = null)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Error message cannot be null or empty.", nameof(error));

        var errorList = errors ?? new List<string>();
        if (!errorList.Contains(error))
        {
            errorList = new List<string>(errorList) { error };
        }

        return new Result<T>(default, false, errorList);
    }

    public static Result<T> Failure(IEnumerable<string> errors)
    {
        if (errors == null)
            throw new ArgumentNullException(nameof(errors));

        var errorList = errors.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
        
        if (errorList.Count == 0)
            throw new ArgumentException("At least one error message is required.", nameof(errors));

        return new Result<T>(default, false, errorList);
    }

    public Result<TOut> Map<TOut>(Func<T, TOut> mapper)
    {
        if (mapper == null)
            throw new ArgumentNullException(nameof(mapper));

        return IsSuccess && Value != null
            ? Result<TOut>.Success(mapper(Value))
            : Result<TOut>.Failure(Errors.Count > 0 ? Errors : new List<string> { "Operation failed" });
    }

    public Result<TOut> FlatMap<TOut>(Func<T, Result<TOut>> binder)
    {
        if (binder == null)
            throw new ArgumentNullException(nameof(binder));

        return IsSuccess && Value != null
            ? binder(Value)
            : Result<TOut>.Failure(Errors.Count > 0 ? Errors : new List<string> { "Operation failed" });
    }

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<string, IReadOnlyList<string>, TOut> onFailure)
    {
        if (onSuccess == null)
            throw new ArgumentNullException(nameof(onSuccess));
        if (onFailure == null)
            throw new ArgumentNullException(nameof(onFailure));

        return IsSuccess && Value != null
            ? onSuccess(Value)
            : onFailure(Errors.FirstOrDefault() ?? "Operation failed", Errors);
    }

    public Result<T> OnSuccess(Action<T> action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (IsSuccess && Value != null)
        {
            action(Value);
        }

        return this;
    }

    public Result<T> OnFailure(Action<string, IReadOnlyList<string>> action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (!IsSuccess)
        {
            action(Errors.FirstOrDefault() ?? "Operation failed", Errors);
        }

        return this;
    }

    public void Deconstruct(out bool isSuccess, out T? value, out IReadOnlyList<string> errors)
    {
        isSuccess = IsSuccess;
        value = Value;
        errors = Errors;
    }

    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }

    public override string ToString()
    {
        return IsSuccess
            ? $"Success: {Value}"
            : $"Failure: {Errors.FirstOrDefault()}{(Errors.Count > 1 ? $" ({Errors.Count} errors)" : "")}";
    }
}

public sealed class Result
{
    public bool IsSuccess { get; }
    public IReadOnlyList<string> Errors { get; }

    private Result(bool isSuccess, IReadOnlyList<string>? errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? Array.Empty<string>();
    }

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(string error, List<string>? errors = null)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Error message cannot be null or empty.", nameof(error));

        var errorList = errors ?? new List<string>();
        if (!errorList.Contains(error))
        {
            errorList = new List<string>(errorList) { error };
        }

        return new Result(false, errorList);
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        if (errors == null)
            throw new ArgumentNullException(nameof(errors));

        var errorList = errors.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
        
        if (errorList.Count == 0)
            throw new ArgumentException("At least one error message is required.", nameof(errors));

        return new Result(false, errorList);
    }

    public Result OnSuccess(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (IsSuccess)
        {
            action();
        }

        return this;
    }

    public Result OnFailure(Action<string, IReadOnlyList<string>> action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (!IsSuccess)
        {
            action(Errors.FirstOrDefault() ?? "Operation failed", Errors);
        }

        return this;
    }

    public void Match(Action onSuccess, Action<string, IReadOnlyList<string>> onFailure)
    {
        if (onSuccess == null)
            throw new ArgumentNullException(nameof(onSuccess));
        if (onFailure == null)
            throw new ArgumentNullException(nameof(onFailure));

        if (IsSuccess)
        {
            onSuccess();
        }
        else
        {
            onFailure(Errors.FirstOrDefault() ?? "Operation failed", Errors);
        }
    }

    public void Deconstruct(out bool isSuccess, out IReadOnlyList<string> errors)
    {
        isSuccess = IsSuccess;
        errors = Errors;
    }

    public override string ToString()
    {
        return IsSuccess
            ? "Success"
            : $"Failure: {Errors.FirstOrDefault()}{(Errors.Count > 1 ? $" ({Errors.Count} errors)" : "")}";
    }
}

