namespace WeatherForecastApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting(RateLimitingPolicies.SlidingWindow)]
[Authorize]
public class WeatherForecastController(IMediator mediator) : ControllerBase
{
    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<ActionResult<Result<GetAllWeatherForecastsResponse>>> Get()
    {
        var query = new GetAllWeatherForecastsQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost(Name = "CreateWeatherForecast")]
    public async Task<ActionResult<Result<CreateWeatherForecastResponse>>> Create([FromBody] WeatherForecastDto dto)
    {
        var command = new CreateWeatherForecastCommand(dto);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("temperature-range/{minTemp}/{maxTemp}", Name = "GetForecastsByTemperatureRange")]
    public async Task<ActionResult<Result<GetForecastsByTemperatureRangeResponse>>> GetByTemperatureRange(
        [FromRoute] int minTemp, 
        [FromRoute] int maxTemp)
    {
        var query = new GetForecastsByTemperatureRangeQuery(minTemp, maxTemp);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
