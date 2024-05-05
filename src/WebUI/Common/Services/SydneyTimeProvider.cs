namespace WebUI.Common.Services;

public class SydneyTimeProvider : TimeProvider
{
    private const string Timezone = "AUS Eastern Standard Time";

    private readonly TimeZoneInfo _sydneyTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Timezone);

    public override TimeZoneInfo LocalTimeZone => _sydneyTimeZone;
}
