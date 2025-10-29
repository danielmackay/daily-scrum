using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using Infrastructure.Graph;
using Infrastructure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mcp.Features.Tasks;

[McpServerToolType]
public class GetTasksTool
{
    private readonly ICurrentUserService _currentUserService;
    private readonly GraphServiceClientFactory _graphServiceClientFactory;
    private readonly ILogger<GraphService> _logger;
    private readonly MsToDoOptions _options;

    public GetTasksTool(
        ICurrentUserService currentUserService, 
        GraphServiceClientFactory graphServiceClientFactory,
        ILogger<GraphService> logger,
        IOptions<MsToDoOptions> options)
    {
        _currentUserService = currentUserService;
        _graphServiceClientFactory = graphServiceClientFactory;
        _logger = logger;
        _options = options.Value;
    }

    [McpServerTool, Description("Get the list of tasks grouped by day and project/list")]
    public async Task<string> GetTasks(
        [Description("Start date in ISO 8601 format (e.g., 2025-10-29T00:00:00Z)")] DateTime utcStart,
        [Description("End date in ISO 8601 format (e.g., 2025-10-29T23:59:59Z)")] DateTime utcEnd)
    {
        // Check if access token is set from environment variable
        var accessToken = !string.IsNullOrEmpty(_options.AccessToken) 
            ? _options.AccessToken 
            : _currentUserService.AccessToken;

        if (string.IsNullOrEmpty(accessToken))
        {
            return JsonSerializer.Serialize(new
            {
                error = "Access token not set. Please set the MSTODO__ACCESSTOKEN environment variable."
            });
        }

        // Update the current user service with the token if it came from options
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
