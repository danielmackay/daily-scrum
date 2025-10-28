using Domain;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using TaskStatus = Domain.TaskStatus;

namespace WebUI.Features.DailyScrumEmail.UseCases.CreateDailyScrumCommand.Infrastructure;

public interface IGraphService
{
    Task<List<Project>> GetTasks(DateTime utcStart, DateTime utcEnd);
    Task<int> GetInboxCount();
}

public class GraphService : IGraphService
{
    private readonly ILogger<GraphService> _logger;
    private readonly GraphServiceClient _graphServiceClient;

    public GraphService(
        ILogger<GraphService> logger,
        GraphServiceClientFactory factory)
    {
        _logger = logger;
        _graphServiceClient = factory.CreateDefault();
    }

    public async Task<List<Project>> GetTasks(DateTime utcStart, DateTime utcEnd)
    {
        _logger.LogInformation("Getting tasks from {UtcStart} to {UtcEnd}", utcStart, utcEnd);

        // NOTE: SHOULD be able to use OData to expand the child tasks, but I haven't been able to get this to work
        var lists = await _graphServiceClient.Me.Todo.Lists.GetAsync();

        //var tasks = new List<Task<TodoTaskCollectionResponse?>>();

        var tasks = new Dictionary<TodoTaskList, Task<TodoTaskCollectionResponse?>>();

        foreach (var list in lists.Value)
        {
            var task = _graphServiceClient.Me.Todo
                .Lists[list.Id]
                .Tasks
                .GetAsync(cfg =>
                {
                    cfg.QueryParameters.Filter =
                        $"LastModifiedDateTime gt {utcStart.ToString("o")} and LastModifiedDateTime lt {utcEnd.ToString("o")}";
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
                    .GroupBy(t => t.Title)
                    .Select(g => g.First())
                    .Select(t => new TaskItem(GetStatus(t.Status), t.Title))
                    .ToList();
                return new Project(title, isSystemList, tasks);
            })
            // .Where(t => t.LastModifiedDateTime?.DateTime.Date == DateTime.UtcNow.Date)
            .Where(p => p.Tasks.Count > 0)
            .ToList();

        return todaysTasks;
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
