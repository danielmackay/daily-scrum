using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WebUI.Features.DailyScrum.Infrastructure;

namespace IntegrationTests;

public class GraphServiceTests
{
    private readonly string? _accessToken;

    public GraphServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<GraphServiceTests>()
            .Build();

        _accessToken = config["MicrosoftGraph:AccessToken"];
    }

    [Fact]
    public async Task CanGetTodaysTasks()
    {
        // Arrange
        var sut = CreateGraphService();
        var today = DateOnly.FromDateTime(DateTime.Now);
        var startOfDayLocal = today.ToDateTime(TimeOnly.MinValue);
        var endOfDayLocal = today.ToDateTime(TimeOnly.MaxValue);
        var startOfDayUtc = startOfDayLocal.ToUniversalTime();
        var endOfDayUtc = endOfDayLocal.ToUniversalTime();

        // Act
        var lists = await sut.GetTasks(startOfDayUtc, endOfDayUtc);
    }

    [Fact]
    public async Task GetInboxCount_ReturnsInboxCount()
    {
        // Arrange
        var sut = CreateGraphService();

        // Act
        var inboxCount = await sut.GetInboxCount();

        // Assert
        inboxCount.Should().Be(84);
    }

    private GraphService CreateGraphService() => new GraphService(CreateOptions());

    private IOptions<MicrosoftGraphOptions> CreateOptions()
    {
        var graphOptions = new MicrosoftGraphOptions
        {
            AccessToken = _accessToken
        };

        return Options.Create(graphOptions);
    }
}
