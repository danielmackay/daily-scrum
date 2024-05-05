using MediatR;
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
    private readonly ILogger<GetTimeSheetNotesQueryHandler> _logger;

    public GetTimeSheetNotesQueryHandler(IGraphService graphService, ILogger<GetTimeSheetNotesQueryHandler> logger)
    {
        _graphService = graphService;
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
        // create a date based on a specific timezone


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
        // Get the Sydney timezone
        var sydneyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");

        // Find the start and end of the day in Sydney time
        var startOfDaySydney = localDate.ToDateTime(TimeOnly.MinValue);
        var endOfDaySydney = localDate.ToDateTime(TimeOnly.MaxValue);

        // Convert the start and end of the day to UTC
        var startOfDayUtc = TimeZoneInfo.ConvertTimeToUtc(startOfDaySydney, sydneyTimeZone);
        var endOfDayUtc = TimeZoneInfo.ConvertTimeToUtc(endOfDaySydney, sydneyTimeZone);

        _logger.LogInformation("Start of day: {StartOfDayUtc}, End of day: {EndOfDayUtc}", startOfDayUtc, endOfDayUtc);

        return (startOfDayUtc, endOfDayUtc);


        // Find the start of the day
        // var startOfDayLocal = localDate.ToDateTime(TimeOnly.MinValue);
        //
        // // Find the end of the day
        // var endOfDayLocal = localDate.ToDateTime(TimeOnly.MaxValue);
        //
        // // Convert to UTC
        // var startOfDayUtc = startOfDayLocal.ToUniversalTime();
        // var endOfDayUtc = endOfDayLocal.ToUniversalTime();
        //
        // return (startOfDayUtc, endOfDayUtc);
    }
}
