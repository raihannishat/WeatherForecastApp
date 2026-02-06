namespace WeatherForecastApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(Result<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<LoginResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var result = await mediator.Send(new LoginCommand(request));
        if (!result.IsSuccess)
            return Unauthorized(result);
        return Ok(result);
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(Result<RegisterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<RegisterResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<RegisterResponse>>> Register([FromBody] RegisterRequest request)
    {
        var result = await mediator.Send(new RegisterCommand(request));
        if (!result.IsSuccess)
            return BadRequest(result);
        return Ok(result);
    }
}
