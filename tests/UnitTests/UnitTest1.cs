using FluentAssertions;
using WebUI.Common.Services;

namespace UnitTests;

public class SydneyTimeProviderTests
{
    [Fact]
    public void GetLocalNow_ReturnsSydneyTime()
    {
        var sut = new SydneyTimeProvider();
        var localNow = DateTimeOffset.Now;
        var result = sut.GetLocalNow();
        result.Should().BeCloseTo(localNow, precision: TimeSpan.FromSeconds(5));
    }
}
