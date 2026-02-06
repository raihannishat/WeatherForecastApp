namespace WeatherForecastApp.UnitTests.TestHelpers;

internal static class DomainTestHelper
{
    public static void SetId(BaseEntity entity, string id)
    {
        var prop = entity.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
        prop!.SetValue(entity, id);
    }
}
