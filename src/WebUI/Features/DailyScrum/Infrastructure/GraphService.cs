using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using WebUI.Features.DailyScrum.Domain;

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
        var graphClient = GetGraphServiceClient();

        // get the user's todo items
        var todoItems = await graphClient.Me.Todo.Lists.GetAsync();

        return todoItems?.Value;
    }

    public async Task<List<TodoTask>?> GetTodoItems()
    {
        var graphClient = GetGraphServiceClient();

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
        var graphClient = GetGraphServiceClient();

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
                    .Select(t => new TaskItem(GetStatus(t.Status), t.Title))
                    .ToList();
                return new Project(title, isSystemList, tasks);
            })
            // .Where(t => t.LastModifiedDateTime?.DateTime.Date == DateTime.UtcNow.Date)
            .Where(p => p.Tasks.Count > 0)
            .ToList();

        return todaysTasks;
    }

    private GraphServiceClient GetGraphServiceClient()
    {
        var credential = new JwtTokenCredential(_options.Value.AccessToken);
        return new GraphServiceClient(credential);
    }

    private Domain.TaskStatus GetStatus(Microsoft.Graph.Models.TaskStatus? status)
    {
        return status switch
        {
            Microsoft.Graph.Models.TaskStatus.Completed => Domain.TaskStatus.Done,
            Microsoft.Graph.Models.TaskStatus.NotStarted => Domain.TaskStatus.Todo,
            Microsoft.Graph.Models.TaskStatus.InProgress => Domain.TaskStatus.InProgress,
            _ => Domain.TaskStatus.Todo,
        };
    }

    public async Task<int> GetInboxCount()
    {
        var graphClient = GetGraphServiceClient();
        var result = await graphClient.Me.MailFolders.GetAsync();

        var inboxCount = result?.Value?.FirstOrDefault(f => f.DisplayName == "Inbox")?.TotalItemCount;

        return inboxCount ?? 0;
    }
}
