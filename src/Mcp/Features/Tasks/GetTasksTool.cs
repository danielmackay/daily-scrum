using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using Infrastructure.Graph;
using Infrastructure.Identity;

namespace Mcp.Features.Tasks;

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
