using Azure.Core;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;

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

    public async Task<List<TodoTask>> GetTasks(DateTime utcStart, DateTime utcEnd)
    {
        var credential = new JwtTokenCredential(_options.Value.AccessToken);

        // create a new instance of the GraphServiceClient
        var graphClient = new GraphServiceClient(credential);

        var lists = await graphClient.Me.Todo.Lists.GetAsync();

        var tasks = new List<Task<TodoTaskCollectionResponse?>>();

        foreach (var list in lists.Value)
        {
            var task = graphClient.Me.Todo
                .Lists[list.Id]
                .Tasks
                .GetAsync(cfg =>
                {
                    cfg.QueryParameters.Filter = $"LastModifiedDateTime gt {utcStart.ToString("o")} and LastModifiedDateTime lt {utcEnd.ToString("o")}";
                });

            tasks.Add(task);
        }

        var result = await Task.WhenAll(tasks);

        var todaysTasks = tasks
            .SelectMany(l => l.Result.Value)
            // .Where(t => t.LastModifiedDateTime?.DateTime.Date == DateTime.UtcNow.Date)
            .ToList();

        return todaysTasks;
    }
}

internal class JwtTokenCredential : TokenCredential
{
    private readonly string accessToken;

    public JwtTokenCredential(string accessToken)
    {
        this.accessToken = accessToken;
    }

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new AccessToken(accessToken, DateTimeOffset.Now.AddDays(1));
    }

    public override ValueTask<AccessToken> GetTokenAsync(
        TokenRequestContext requestContext,
        CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(GetToken(requestContext, cancellationToken));
    }
}
