namespace WebUI.Common.Services;

// Problem: Issue is that DateTime.Now could be different depending on where the code is running
// Solution: Force 'now' to be based on AEST, then converted to UTC

public class SydneyTimeProvider : TimeProvider
{
    private const string Timezone = "AUS Eastern Standard Time";

    private readonly TimeZoneInfo _sydneyTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Timezone);

    public override TimeZoneInfo LocalTimeZone => _sydneyTimeZone;
}

public static class TimeProviderExt
{
    public static DateOnly GetToday(this TimeProvider timeProvider)
    {
        var now = timeProvider.GetLocalNow();
        return DateOnly.FromDateTime(now.Date);
    }

    public static DateTime GetStartOfDayUtc(this TimeProvider timeProvider, DateOnly date)
    {
        var timeZone = timeProvider.LocalTimeZone;

        // Find the start of the day in Sydney time
        var startOfDaySydney = date.ToDateTime(TimeOnly.MinValue);

        // Convert the start of the day to UTC
        var startOfDayUtc = TimeZoneInfo.ConvertTimeToUtc(startOfDaySydney, timeZone);

        return startOfDayUtc;
    }

    public static DateTime GetEndOfDayUtc(this TimeProvider timeProvider, DateOnly date)
    {
        var timeZone = timeProvider.LocalTimeZone;

        // Find the start of the day in Sydney time
        var endOfDaySydney = date.ToDateTime(TimeOnly.MaxValue);

        // Convert the start of the day to UTC
        var endOfDayUtc = TimeZoneInfo.ConvertTimeToUtc(endOfDaySydney, timeZone);

        return endOfDayUtc;
    }
}
