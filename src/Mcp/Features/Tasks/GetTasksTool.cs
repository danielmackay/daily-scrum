using Domain.Time;
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
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<GraphService> _logger;
    private readonly MsToDoOptions _options;

    public GetTasksTool(
        ICurrentUserService currentUserService, 
        GraphServiceClientFactory graphServiceClientFactory,
        TimeProvider timeProvider,
        ILogger<GraphService> logger,
        IOptions<MsToDoOptions> options)
    {
        _currentUserService = currentUserService;
        _graphServiceClientFactory = graphServiceClientFactory;
        _timeProvider = timeProvider;
        _logger = logger;
        _options = options.Value;
    }

    [McpServerTool, Description("Get the list of tasks grouped by day and project/list")]
    public async Task<string> GetTasks(
        [Description("Local Start date in ISO 8601 format (e.g., 2025-10-29T00:00:00Z)")] DateTime localStart,
        [Description("Local End date in ISO 8601 format (e.g., 2025-10-29T23:59:59Z)")] DateTime localEnd)
    {
        // Check if access token is set from environment variable
        var accessToken = !string.IsNullOrEmpty(_options.AccessToken) 
            ? _options.AccessToken 
            : _currentUserService.AccessToken;

        if (string.IsNullOrEmpty(accessToken))
        {
            var errorResponse = new GetTasksErrorResponse
            {
                Error = "Access token not set. Please set the MSTODO__ACCESSTOKEN environment variable."
            };
            return JsonSerializer.Serialize(errorResponse);
        }

        // Update the current user service with the token if it came from options
        _currentUserService.UpdateAccessToken(accessToken);

        // Create a new GraphService using the GraphServiceClientFactory
        var graphService = new GraphService(_logger, _graphServiceClientFactory);

        var startOfDayUtc = _timeProvider.GetStartOfDayUtc(DateOnly.FromDateTime(localStart));
        var endOfDayUtc = _timeProvider.GetEndOfDayUtc(DateOnly.FromDateTime(localEnd));

        var tasksByDay = await graphService.GetTasksByDay(startOfDayUtc, endOfDayUtc, _timeProvider);

        return JsonSerializer.Serialize(tasksByDay, new JsonSerializerOptions { WriteIndented = true });
    }
}
