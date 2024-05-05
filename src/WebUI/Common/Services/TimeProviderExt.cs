namespace WebUI.Common.Services;

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