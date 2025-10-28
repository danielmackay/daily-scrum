using Domain;
using FluentAssertions;

namespace UnitTests;

public class DailyScrumTests
{
    [Fact]
    public void CanSerializeDailyScrum()
    {
        // Arrange
        var dailyScrum = CreateDailyScrum();
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(dailyScrum);

        // Act
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<DailyScrum>(json);

        // Assert
        result.Should().NotBeNull();
    }

    private Domain.DailyScrum CreateDailyScrum()
    {
        var userSummary = new UserSummary(1, 2);
        var yesterday = new ProjectList([]);
        var today = new ProjectList([]);
        var emailSummary = new EmailSummary("foo");
        return new Domain.DailyScrum(userSummary, yesterday, today, emailSummary);
    }
}
