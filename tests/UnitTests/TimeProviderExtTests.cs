using FluentAssertions;
using WebUI.Common.Services;

namespace UnitTests;

public class TimeProviderExtTests
{
    [Fact]
    public void GetStartOfDayUtc_GivenSydneyTimeZone_ReturnsCorrectUtc()
    {
        var sut = new SydneyTimeProvider();
        var date = new DateOnly(2024, 5, 5);
        var expected = new DateTime(2024, 5, 4, 14, 0, 0, DateTimeKind.Utc);

        var result = sut.GetStartOfDayUtc(date);

        result.Should().Be(expected);
    }

    [Fact]
    public void GetEndOfDayUtc_GivenSydneyTimeZone_ReturnsCorrectUtc()
    {
        var sut = new SydneyTimeProvider();
        var date = new DateOnly(2024, 5, 5);
        var expected = new DateTime(2024, 5, 5, 13, 59, 59, DateTimeKind.Utc);

        var result = sut.GetEndOfDayUtc(date);

        result.Should().BeCloseTo(expected, TimeSpan.FromSeconds(1));
    }






}