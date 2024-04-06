using Microsoft.Extensions.Options;
using WebUI.Features.DailyScrum.Infrastructure;

namespace IntegrationTests;

public class GraphServiceTests
{
    [Fact]
    public async Task CanGetTodoLists()
    {
        var sut = CreateGraphService();
        var lists = await sut.GetTodoLists();
    }

    [Fact]
    public async Task CanGetTodoItems()
    {
        var sut = CreateGraphService();
        var lists = await sut.GetTodoItems();
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

    private GraphService CreateGraphService() => new GraphService(CreateOptions());

    private IOptions<MicrosoftGraphOptions> CreateOptions()
    {
        var graphOptions = new MicrosoftGraphOptions
        {
            AccessToken = ""
        };

        return Options.Create(graphOptions);
    }
}
