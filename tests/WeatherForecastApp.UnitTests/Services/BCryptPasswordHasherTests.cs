namespace WeatherForecastApp.UnitTests.Services;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _sut = new();

    [Fact]
    public void Hash_ReturnsNonEmptyHash()
    {
        var hash = _sut.Hash("password123");
        Assert.False(string.IsNullOrEmpty(hash));
        Assert.NotEqual("password123", hash);
    }

    [Fact]
    public void Hash_SamePasswordProducesDifferentHashes()
    {
        var hash1 = _sut.Hash("password123");
        var hash2 = _sut.Hash("password123");
        Assert.NotEqual(hash1, hash2); // different salt each time
    }

    [Fact]
    public void Verify_WhenPasswordMatches_ReturnsTrue()
    {
        var hash = _sut.Hash("password123");
        Assert.True(_sut.Verify("password123", hash));
    }

    [Fact]
    public void Verify_WhenPasswordDoesNotMatch_ReturnsFalse()
    {
        var hash = _sut.Hash("password123");
        Assert.False(_sut.Verify("wrongpassword", hash));
    }
}
