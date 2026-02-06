namespace WeatherForecastApp.API.Constants;

public static class RateLimitingPolicies
{
    public const string FixedWindow = "FixedWindowPolicy";
    public const string SlidingWindow = "SlidingWindowPolicy";
    public const string TokenBucket = "TokenBucketPolicy";
}

