using MediatR;
using WebUI.Common.Services;
using WebUI.Common.ViewModels;
using WebUI.Features.DailyScrum.Infrastructure;
using WebUI.Features.DailyScrum.Queries;

namespace WebUI.Features.Timesheet.Queries;

// How should the time calculation work?
// - user enters a date
// - we assume that that date is in Sydney time
//
public record GetTimeSheetNotesQuery(DateOnly Date) : IRequest<TimesheetViewModel>;

public class GetTimeSheetNotesQueryHandler : IRequestHandler<GetTimeSheetNotesQuery, TimesheetViewModel>
{
    private readonly IGraphService _graphService;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<GetTimeSheetNotesQueryHandler> _logger;

    public GetTimeSheetNotesQueryHandler(IGraphService graphService, TimeProvider timeProvider, ILogger<GetTimeSheetNotesQueryHandler> logger)
    {
        _graphService = graphService;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<TimesheetViewModel> Handle(GetTimeSheetNotesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting timesheet notes for {Date}", request.Date);

        var projects = await GetProjects(request.Date);

        return new TimesheetViewModel
        {
            Projects = projects
        };
    }

    // TODO: Consider refactoring into a common service
    private async Task<List<ProjectViewModel>> GetProjects(DateOnly date)
    {
        var startOfDayUtc = _timeProvider.GetStartOfDayUtc(date);
        var endOfDayUtc = _timeProvider.GetEndOfDayUtc(date);

        _logger.LogInformation("Getting projects for {Date} ({StartOfDayUtc} to {EndOfDayUtc})", date, startOfDayUtc, endOfDayUtc);

        var graphTasks = await _graphService.GetTasks(startOfDayUtc, endOfDayUtc);

        var projects = graphTasks
            .Select(p => new ProjectViewModel
            {
                Name = p.Name,
                IsSystemProject = p.IsSystemProject,
                Tasks = p.Tasks
                    .Select(t => new TaskViewModel { Name = t.Name })
                    .ToList()
            })
            .ToList();

        return projects;
    }
}
