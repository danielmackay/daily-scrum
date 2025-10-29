using ModelContextProtocol.Server;
using System.ComponentModel;
using Infrastructure.Graph;
using Infrastructure.Identity;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.AddConsole();

// Add services
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<GraphServiceClientFactory>();
builder.Services.AddScoped<IGraphService, GraphService>();

// Add MCP server services
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Map MCP server endpoints
app.MapMcp();

app.Run();

[McpServerToolType]
public static class HelloWorldTool
{
    [McpServerTool, Description("A simple greeting tool that says hello to the specified name")]
    public static string SayHello([Description("The name of the person to greet")] string name)
    {
        return $"Hello, {name}! Welcome to the MCP server.";
    }
}

[McpServerToolType]
public class GetTasksTool
{
    private readonly ICurrentUserService _currentUserService;
    private readonly GraphServiceClientFactory _graphServiceClientFactory;
    private readonly ILogger<GraphService> _logger;

    public GetTasksTool(
        ICurrentUserService currentUserService, 
        GraphServiceClientFactory graphServiceClientFactory,
        ILogger<GraphService> logger)
    {
        _currentUserService = currentUserService;
        _graphServiceClientFactory = graphServiceClientFactory;
        _logger = logger;
    }

    [McpServerTool, Description("Get the list of tasks grouped by day and project/list")]
    public async Task<string> GetTasks(
        [Description("Access token for authentication")] string accessToken,
        [Description("Start date in ISO 8601 format (e.g., 2025-10-29T00:00:00Z)")] DateTime utcStart,
        [Description("End date in ISO 8601 format (e.g., 2025-10-29T23:59:59Z)")] DateTime utcEnd)
    {
        // Update the current user service with the access token
        _currentUserService.UpdateAccessToken(accessToken);

        // Create a new GraphService using the GraphServiceClientFactory
        var graphService = new GraphService(_logger, _graphServiceClientFactory);

        var projects = await graphService.GetTasks(utcStart, utcEnd);

        // Group tasks by day and project
        var result = new
        {
            startDate = utcStart,
            endDate = utcEnd,
            tasksByDay = new[]
            {
                new
                {
                    date = utcStart.Date,
                    projects = projects.Select(p => new
                    {
                        name = p.Name,
                        isSystemProject = p.IsSystemProject,
                        tasks = p.Tasks.Select(t => new
                        {
                            id = t.Id,
                            name = t.Name,
                            displayName = t.DisplayName,
                            status = t.Status.ToString()
                        })
                    })
                }
            }
        };

        return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }
}