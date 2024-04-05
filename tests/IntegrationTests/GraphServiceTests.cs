using WebUI.Features.DailyScrum.Infrastructure;

namespace IntegrationTests;

public class GraphServiceTests
{


    [Fact]
    public async Task CanGetTodoLists()
    {
        var sut = new GraphService();
        var lists = await sut.GetTodoLists();
    }

    [Fact]
    public async Task CanGetTodoItems()
    {
        var sut = new GraphService();
        var lists = await sut.GetTodoItems();
    }

    [Fact]
    public async Task CanGetTodaysTasks()
    {
        var sut = new GraphService();
        var lists = await sut.GetTodaysTasks();
    }
}
