using Azure.Identity;
using Domain.Time;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using Infrastructure.Graph;
using Microsoft.Extensions.Logging;

namespace Mcp.Features.Tasks;

[McpServerToolType]
public class GetTasksTool
{
    private readonly GraphServiceClientFactory _graphServiceClientFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<GraphService> _logger;

    public GetTasksTool(
        GraphServiceClientFactory graphServiceClientFactory,
        TimeProvider timeProvider,
        ILogger<GraphService> logger)
    {
        _graphServiceClientFactory = graphServiceClientFactory;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    [McpServerTool, Description("Get the list of tasks grouped by day and project/list")]
    public async Task<string> GetTasks(
        [Description("Local Start date in ISO 8601 format (e.g., 2025-10-29T00:00:00Z)")] DateTime localStart,
        [Description("Local End date in ISO 8601 format (e.g., 2025-10-29T23:59:59Z)")] DateTime localEnd)
    {
        try
        {
            // Token comes from the shared, silently-refreshed cache seeded by
            // `daily-scrum login` — no access token is passed in or read from env.
            var graphService = GraphService.FromClient(
                _logger, _graphServiceClientFactory.CreateWithCachedCredential());

            var startOfDayUtc = _timeProvider.GetStartOfDayUtc(DateOnly.FromDateTime(localStart));
            var endOfDayUtc = _timeProvider.GetEndOfDayUtc(DateOnly.FromDateTime(localEnd));

            var tasksByDay = await graphService.GetTasksByDay(startOfDayUtc, endOfDayUtc, _timeProvider);

            return JsonSerializer.Serialize(tasksByDay, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex) when (ex is AuthenticationRequiredException or CredentialUnavailableException)
        {
            _logger.LogWarning(ex, "Graph authentication required; user must sign in again");
            var errorResponse = new GetTasksErrorResponse
            {
                Error = "Not authenticated. Run 'daily-scrum login' in a terminal, then try again."
            };
            return JsonSerializer.Serialize(errorResponse);
        }
    }
}
