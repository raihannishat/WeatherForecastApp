namespace WeatherForecastApp.Application.Features.WeatherForecasts.Commands.CreateWeatherForecast;

public class CreateWeatherForecastCommandHandler(
    IWeatherForecastService service,
    IMapper mapper) 
    : IRequestHandler<CreateWeatherForecastCommand, Result<CreateWeatherForecastResponse>>
{
    public async Task<Result<CreateWeatherForecastResponse>> Handle(CreateWeatherForecastCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = await service.CreateForecastAsync(request.Dto);
            
            // Use object mapper instead of manual copying
            var response = mapper.Map<WeatherForecastDto, CreateWeatherForecastResponse>(dto);
            response.Message = "Weather forecast created successfully";

            return Result<CreateWeatherForecastResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CreateWeatherForecastResponse>.Failure(ex.Message);
        }
    }
}