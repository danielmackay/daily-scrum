using Domain;
using Domain.Time;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Extensions.Logging;
using TaskStatus = Domain.TaskStatus;

namespace Infrastructure.Graph;

public interface IGraphService
{
    Task<List<Project>> GetTasks(DateTime utcStart, DateTime utcEnd);
    Task<List<TasksByDay>> GetTasksByDay(DateTime utcStart, DateTime utcEnd, TimeProvider timeProvider);
    Task<int> GetInboxCount();
}

public class GraphService : IGraphService
{
    private readonly ILogger<GraphService> _logger;
    private readonly GraphServiceClient _graphServiceClient;

    public GraphService(
        ILogger<GraphService> logger,
        GraphServiceClientFactory factory)
        : this(logger, factory.CreateWithAccessToken())
    {
    }

    // Private so the DI container only ever sees the single public (factory-based)
    // constructor — adding a second public 2-arg ctor would make WebUI's container
    // resolution ambiguous, since AddMicrosoftGraph also registers a GraphServiceClient.
    private GraphService(ILogger<GraphService> logger, GraphServiceClient graphServiceClient)
    {
        _logger = logger;
        _graphServiceClient = graphServiceClient;
    }

    // Entry point for the MCP server: supply a client built from the shared,
    // silently-refreshed token cache (GraphServiceClientFactory.CreateWithCachedCredential).
    public static GraphService FromClient(ILogger<GraphService> logger, GraphServiceClient client)
        => new(logger, client);

    public async Task<List<Project>> GetTasks(DateTime utcStart, DateTime utcEnd)
    {
        _logger.LogInformation("Getting tasks from {UtcStart} to {UtcEnd}", utcStart, utcEnd);

        // NOTE: SHOULD be able to use OData to expand the child tasks, but I haven't been able to get this to work
        var lists = await _graphServiceClient.Me.Todo.Lists.GetAsync();

        //var tasks = new List<Task<TodoTaskCollectionResponse?>>();
        if (lists?.Value is null)
            return [];

        var tasks = new Dictionary<TodoTaskList, Task<TodoTaskCollectionResponse?>>();

        foreach (var list in lists.Value)
        {
            var task = _graphServiceClient.Me.Todo
                .Lists[list.Id]
                .Tasks
                .GetAsync(cfg =>
                {
                    cfg.QueryParameters.Filter =
                        $"LastModifiedDateTime gt {utcStart:o} and LastModifiedDateTime lt {utcEnd:o}";
                });

            tasks.Add(list, task);
        }

        _ = await Task.WhenAll(tasks.Values);

        var todaysTasks = tasks
            .Select(kvp =>
            {
                var title = kvp.Key.DisplayName ?? "Unnamed List";
                var isSystemList = kvp.Key.WellknownListName != WellknownListName.None;
                var taskItems = kvp.Value.Result?.Value == null ?
                    [] :
                    kvp.Value.Result.Value
                    .GroupBy(t => t.Title)
                    .Select(g => g.First())
                    .Select(t => new TaskItem(GetStatus(t.Status), t.Title ?? "Untitled Task"))
                    .ToList();
                return new Project(title, isSystemList, taskItems);

            })
            // .Where(t => t.LastModifiedDateTime?.DateTime.Date == DateTime.UtcNow.Date)
            .Where(p => p.Tasks.Count > 0)
            .ToList();

        return todaysTasks;
    }

    public async Task<List<TasksByDay>> GetTasksByDay(DateTime utcStart, DateTime utcEnd, TimeProvider timeProvider)
    {
        _logger.LogInformation("Getting tasks grouped by day from {UtcStart} to {UtcEnd}", utcStart, utcEnd);

        var lists = await _graphServiceClient.Me.Todo.Lists.GetAsync();

        if (lists?.Value is null)
            return [];

        var tasks = new Dictionary<TodoTaskList, Task<TodoTaskCollectionResponse?>>();

        foreach (var list in lists.Value)
        {
            var task = _graphServiceClient.Me.Todo
                .Lists[list.Id]
                .Tasks
                .GetAsync(cfg =>
                {
                    cfg.QueryParameters.Filter =
                        $"LastModifiedDateTime gt {utcStart:o} and LastModifiedDateTime lt {utcEnd:o}";
                });

            tasks.Add(list, task);
        }

        _ = await Task.WhenAll(tasks.Values);

        // Group all tasks by day
        var tasksByDay = new Dictionary<DateOnly, List<Project>>();

        foreach (var kvp in tasks)
        {
            var listName = kvp.Key.DisplayName ?? "Unnamed List";
            var isSystemList = kvp.Key.WellknownListName != WellknownListName.None;
            var todoTasks = kvp.Value.Result?.Value;

            if (todoTasks is null)
                continue;

            // Group tasks from this list by their last modified date
            var tasksGroupedByDay = todoTasks
                .Where(t => t.LastModifiedDateTime.HasValue)
                .GroupBy(t => timeProvider.GetLocalDate(t.LastModifiedDateTime!.Value))
                .ToList();

            foreach (var dayGroup in tasksGroupedByDay)
            {
                var date = dayGroup.Key;
                
                var taskItems = dayGroup
                    .GroupBy(t => t.Title)
                    .Select(g => g.First())
                    .Select(t => new TaskItem(GetStatus(t.Status), t.Title ?? "Untitled Task"))
                    .ToList();

                if (taskItems.Count == 0)
                    continue;

                if (!tasksByDay.ContainsKey(date))
                {
                    tasksByDay[date] = new List<Project>();
                }

                tasksByDay[date].Add(new Project(listName, isSystemList, taskItems));
            }
        }

        // Convert to list of TasksByDay and sort by date
        var result = tasksByDay
            .Select(kvp => new TasksByDay(kvp.Key, kvp.Value))
            .OrderBy(t => t.Date)
            .ToList();

        return result;
    }


    private TaskStatus GetStatus(Microsoft.Graph.Models.TaskStatus? status)
    {
        return status switch
        {
            Microsoft.Graph.Models.TaskStatus.Completed => TaskStatus.Done,
            Microsoft.Graph.Models.TaskStatus.NotStarted => TaskStatus.Todo,
            Microsoft.Graph.Models.TaskStatus.InProgress => TaskStatus.InProgress,
            _ => TaskStatus.Todo,
        };
    }

    public async Task<int> GetInboxCount()
    {
        try
        {
            var result = await _graphServiceClient.Me.MailFolders.GetAsync();

            var inboxCount = result?.Value?.FirstOrDefault(f => f.DisplayName == "Inbox")?.TotalItemCount;

            return inboxCount ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inbox count");
            return 0;
        }
    }
}
