namespace WeatherForecastApp.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (code, errorResponse) = exception switch
        {
            ValidationException validationException => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    Error = "Validation Error",
                    Message = "One or more validation errors occurred",
                    Errors = validationException.Errors.Select(e => new ValidationError
                    {
                        Property = e.PropertyName,
                        Message = e.ErrorMessage,
                        AttemptedValue = e.AttemptedValue
                    }).ToList()
                }
            ),
            Domain.Exceptions.DomainException domainException => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    Error = "Domain Error",
                    Message = domainException.Message
                }
            ),
            InvalidOperationException or ArgumentException => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    Error = "Bad Request",
                    Message = exception.Message
                }
            ),
            KeyNotFoundException or FileNotFoundException => (
                HttpStatusCode.NotFound,
                new ErrorResponse
                {
                    Error = "Not Found",
                    Message = exception.Message
                }
            ),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                new ErrorResponse
                {
                    Error = "Unauthorized",
                    Message = exception.Message
                }
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse
                {
                    Error = "Internal Server Error",
                    Message = exception.Message
                }
            )
        };

        var result = JsonSerializer.Serialize(errorResponse);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}

