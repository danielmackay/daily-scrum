using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using WebUI.Common.Identity;
using WebUI.Features.DailyScrum.Domain;

namespace WebUI.Features.DailyScrum.Infrastructure;

public interface IGraphService
{
    Task<List<Project>> GetTasks(DateTime utcStart, DateTime utcEnd);
    Task<int> GetInboxCount();
}

public class GraphService : IGraphService
{
    private readonly string _clientId = "2407f45c-4141-4484-8fc5-ce61327519d9";
    private readonly string _tenantId = "ac2f7c34-b935-48e9-abdc-11e5d4fcb2b0";
    private readonly string _redirectUri = "http://localhost:5001"; // Must match redirect URI in app registration
    private readonly string[] _scopes = new[] { "User.Read" }; // Define scopes you need

    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GraphService> _logger;

    public GraphService(ICurrentUserService currentUserService, ILogger<GraphService> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    // public async Task<List<TodoTaskList>?> GetTodoLists()
    // {
    //     var graphClient = GetGraphServiceClient();
    //
    //     // get the user's todo items
    //     var todoItems = await graphClient.Me.Todo.Lists.GetAsync();
    //
    //     return todoItems?.Value;
    // }
    //
    // public async Task<List<TodoTask>?> GetTodoItems()
    // {
    //     var graphClient = GetGraphServiceClient();
    //
    //     // get the user's todo items
    //     var todoItems = await graphClient.Me.Todo
    //         .Lists[
    //             "AAMkADc2YTU0YjZhLWQ5YjMtNGEyMS04MjBhLTZiMmE5NTYyMGIzYQAuAAAAAACP6decNu2DQYGmhrqvh_OSAQCUGIMeUnEkQY4T_KIyV7H1AADdl5LEAAA="]
    //         .Tasks
    //         .GetAsync();
    //     return todoItems?.Value;
    // }

    public async Task<List<Project>> GetTasks(DateTime utcStart, DateTime utcEnd)
    {
        _logger.LogInformation("Getting tasks from {UtcStart} to {UtcEnd}", utcStart, utcEnd);

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

    private GraphServiceClient GetGraphServiceClient()
    {
        var accessToken = _currentUserService.AccessToken;
        ArgumentException.ThrowIfNullOrEmpty(accessToken);
        var credential = new JwtTokenCredential(accessToken);
        return new GraphServiceClient(credential);
    }

    public async Task<GraphServiceClient> GetAuthenticatedGraphClientAsync()
    {
        var scopes = new[] { "User.Read" };

        // Multi-tenant apps can use "common",
        // single-tenant apps must use the tenant ID from the Azure portal
        // var tenantId = "common";

// Value from app registration
        // var clientId = "YOUR_CLIENT_ID";

// using Azure.Identity;
        var options = new DeviceCodeCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            ClientId = _clientId,
            TenantId = _tenantId,
            // Callback function that receives the user prompt
            // Prompt contains the generated device code that user must
            // enter during the auth process in the browser
            DeviceCodeCallback = (code, cancellation) =>
            {
                Console.WriteLine(code.Message);
                return Task.FromResult(0);
            },
        };

// https://learn.microsoft.com/dotnet/api/azure.identity.devicecodecredential
        var deviceCodeCredential = new DeviceCodeCredential(options);

        var graphClient = new GraphServiceClient(deviceCodeCredential, scopes);

        return graphClient;
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
        try
        {
            var graphClient = await GetAuthenticatedGraphClientAsync();
            var result = await graphClient.Me.MailFolders.GetAsync();

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
