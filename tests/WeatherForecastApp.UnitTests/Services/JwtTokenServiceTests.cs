namespace WeatherForecastApp.UnitTests.Services;

public class JwtTokenServiceTests
{
    private const string Secret = "TestSecretKeyThatIsAtLeast32CharactersLong!";

    [Fact]
    public void GenerateToken_ReturnsNonEmptyToken()
    {
        var options = Options.Create(new JwtSettings
        {
            Secret = Secret,
            Issuer = "Test",
            Audience = "Test",
            ExpirationMinutes = 60
        });
        var sut = new JwtTokenService(options);

        var token = sut.GenerateToken("user-1", "a@b.com");

        Assert.False(string.IsNullOrEmpty(token));
        Assert.Contains(".", token); // JWT has 3 parts
    }

    [Fact]
    public void GenerateToken_ReturnsDifferentTokensForDifferentCalls()
    {
        var options = Options.Create(new JwtSettings
        {
            Secret = Secret,
            Issuer = "Test",
            Audience = "Test",
            ExpirationMinutes = 60
        });
        var sut = new JwtTokenService(options);

        var token1 = sut.GenerateToken("user-1", "a@b.com");
        var token2 = sut.GenerateToken("user-1", "a@b.com");

        Assert.NotEqual(token1, token2); // Jti claim is different each time
    }

    [Fact]
    public void GenerateToken_WithDifferentUserId_ReturnsDifferentTokens()
    {
        var options = Options.Create(new JwtSettings
        {
            Secret = Secret,
            Issuer = "Test",
            Audience = "Test",
            ExpirationMinutes = 60
        });
        var sut = new JwtTokenService(options);

        var token1 = sut.GenerateToken("user-1", "a@b.com");
        var token2 = sut.GenerateToken("user-2", "a@b.com");

        Assert.NotEqual(token1, token2);
    }
}
