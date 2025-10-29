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

        var tasksByDay = await graphService.GetTasksByDay(utcStart, utcEnd);

        // var result = new GetTasksResponse
        // {
        //     StartDate = utcStart,
        //     EndDate = utcEnd,
        //     TasksByDay = tasksByDay.Select(tbd => new Mcp.Features.Tasks.TasksByDay
        //     {
        //         Date = tbd.Date,
        //         Projects = tbd.Projects.Select(p => new ProjectTasks
        //         {
        //             Name = p.Name,
        //             IsSystemProject = p.IsSystemProject,
        //             Tasks = p.Tasks.Select(t => new TaskDetails
        //             {
        //                 Id = t.Id,
        //                 Name = t.Name,
        //                 DisplayName = t.DisplayName,
        //                 Status = t.Status.ToString()
        //             }).ToList()
        //         }).ToList()
        //     }).ToList()
        // };

        return JsonSerializer.Serialize(tasksByDay, new JsonSerializerOptions { WriteIndented = true });
    }
}
