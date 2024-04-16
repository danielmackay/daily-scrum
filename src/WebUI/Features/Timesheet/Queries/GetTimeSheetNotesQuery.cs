using MediatR;
using WebUI.Common.ViewModels;
using WebUI.Features.DailyScrum.Infrastructure;
using WebUI.Features.DailyScrum.Queries;

namespace WebUI.Features.Timesheet.Queries;

public record GetTimeSheetNotesQuery(DateOnly Date) : IRequest<TimesheetViewModel>;

public class GetTimeSheetNotesQueryHandler : IRequestHandler<GetTimeSheetNotesQuery, TimesheetViewModel>
{
    private readonly IGraphService _graphService;

    public GetTimeSheetNotesQueryHandler(IGraphService graphService)
    {
        _graphService = graphService;
    }

    public async Task<TimesheetViewModel> Handle(GetTimeSheetNotesQuery request, CancellationToken cancellationToken)
    {
        var projects = await GetProjects(request.Date);

        return new TimesheetViewModel
        {
            Projects = projects
        };
    }

    // TODO: Consider refactoring into a common service
    private async Task<List<ProjectViewModel>> GetProjects(DateOnly date)
    {
        var (startOfDayUtc, endOfDayUtc) = GetTimeStamps(date);

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

    // TODO: Consider refactoring into a common service
    private (DateTime StartOfDayUtc, DateTime EndOfDayUtc) GetTimeStamps(DateOnly localDate)
    {
        // Find the start of the day
        var startOfDayLocal = localDate.ToDateTime(TimeOnly.MinValue);

        // Find the end of the day
        var endOfDayLocal = localDate.ToDateTime(TimeOnly.MaxValue);

        // Convert to UTC
        var startOfDayUtc = startOfDayLocal.ToUniversalTime();
        var endOfDayUtc = endOfDayLocal.ToUniversalTime();

        return (startOfDayUtc, endOfDayUtc);
    }
}
