using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebUI.Common.Identity;
using WebUI.Common.Services;
using WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand;
using WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand.Infrastructure;

namespace IntegrationTests;

public class GraphServiceTests
{
    private readonly string _accessToken;

    public GraphServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<GraphServiceTests>()
            .Build();

        _accessToken = config["MicrosoftGraph:AccessToken"]!;
        ArgumentException.ThrowIfNullOrEmpty(_accessToken);
    }

    [Fact]
    public async Task CanGetTodaysTasks()
    {
        // Arrange
        var sut = CreateGraphService();
        var timeProvider = new SydneyTimeProvider();
        var today = timeProvider.GetToday();
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

    private GraphService CreateGraphService()
    {
        var userService = new CurrentUserService();
        userService.UpdateAccessToken(_accessToken);
        var logger = new Logger<GraphService>(new LoggerFactory());
        var factory = new GraphServiceClientFactory(userService, new ServiceCollection().BuildServiceProvider());
        var service = new GraphService(logger, factory);
        return service;
    }
}
