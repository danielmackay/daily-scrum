using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using WebUI.Features.DailyScrum.Domain;
using TaskStatus = WebUI.Features.DailyScrum.Domain.TaskStatus;

namespace WebUI.Features.DailyScrum.Infrastructure;

public class GraphService
{
    private readonly IOptions<MicrosoftGraphOptions> _options;

    public GraphService(IOptions<MicrosoftGraphOptions> options)
    {
        _options = options;
    }

    public async Task<List<TodoTaskList>?> GetTodoLists()
    {
        var credential = new JwtTokenCredential(_options.Value.AccessToken);

        // create a new instance of the GraphServiceClient
        var graphClient = new GraphServiceClient(credential);

        // get the user's todo items
        var todoItems = await graphClient.Me.Todo.Lists.GetAsync();

        return todoItems?.Value;
    }

    public async Task<List<TodoTask>?> GetTodoItems()
    {
        var credential = new JwtTokenCredential(_options.Value.AccessToken);

        // create a new instance of the GraphServiceClient
        var graphClient = new GraphServiceClient(credential);

        // get the user's todo items
        var todoItems = await graphClient.Me.Todo
            .Lists[
                "AAMkADc2YTU0YjZhLWQ5YjMtNGEyMS04MjBhLTZiMmE5NTYyMGIzYQAuAAAAAACP6decNu2DQYGmhrqvh_OSAQCUGIMeUnEkQY4T_KIyV7H1AADdl5LEAAA="]
            .Tasks
            .GetAsync();
        return todoItems?.Value;
    }

    public async Task<List<Project>> GetTasks(DateTime utcStart, DateTime utcEnd)
    {
        var credential = new JwtTokenCredential(_options.Value.AccessToken);
        var graphClient = new GraphServiceClient(credential);

        // NOTE: SHOULD be able to use OData to expand the child tasks, but I haven't been able to get this to work
        var lists = await graphClient.Me.Todo.Lists.GetAsync();

        //var tasks = new List<Task<TodoTaskCollectionResponse?>>();

        var tasks = new Dictionary<TodoTaskList, Task<TodoTaskCollectionResponse?>>();


        foreach (var list in lists.Value)
        {
            var task = graphClient.Me.Todo
                .Lists[list.Id]
                .Tasks
                .GetAsync(cfg =>
                {
                    cfg.QueryParameters.Filter = $"LastModifiedDateTime gt {utcStart.ToString("o")} and LastModifiedDateTime lt {utcEnd.ToString("o")}";
                });

            tasks.Add(list, task);
        }

        var result = await Task.WhenAll(tasks.Values);

        var todaysTasks = tasks
            .Select(kvp =>
            {
                var title = kvp.Key.DisplayName;
                var isSystemList = kvp.Key.WellknownListName != WellknownListName.None;
                var tasks = kvp.Value.Result.Value
                    .Select(t => new TaskItem(TaskStatus.Done, t.Title))
                    .ToList();
                return new Project(title, isSystemList, tasks);
            })
            // .Where(t => t.LastModifiedDateTime?.DateTime.Date == DateTime.UtcNow.Date)
            .Where(p => p.Tasks.Count > 0)
            .ToList();

        return todaysTasks;
    }
}
