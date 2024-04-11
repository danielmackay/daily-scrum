using MediatR;
using Microsoft.Graph.Reports.GetEmailActivityCountsWithPeriod;
using WebUI.Features.DailyScrum.Infrastructure;

namespace WebUI.Features.DailyScrum.Queries;

public class GetDailyScrumQuery : IRequest<DailyScrumViewModel>;

public class GetDailyScrumQueryHandler : IRequestHandler<GetDailyScrumQuery, DailyScrumViewModel>
{
    private readonly IGraphService _graphService;

    public GetDailyScrumQueryHandler(IGraphService graphService)
    {
        _graphService = graphService;
    }

    public async Task<DailyScrumViewModel> Handle(GetDailyScrumQuery request, CancellationToken cancellationToken)
    {
        var email = GetEmail();

        var userSummary = await GetUserSummary();

        var today = GetToday();
        var todaysProjects = await GetProjects(today);

        var yesterday = GetLastWorkingDay();
        var yesterdaysProjects = await GetProjects(yesterday);

        return new DailyScrumViewModel
        {
            UserSummary = userSummary,
            TodaysProjects = todaysProjects,
            YesterdaysProjects = yesterdaysProjects,
            Email = email
        };
    }

    private async Task<UserSummaryViewModel> GetUserSummary()
    {
        var inboxCount = await _graphService.GetInboxCount();

        return new UserSummaryViewModel
        {
            DaysUntilNextBooking = "♾️",
            InboxCount = inboxCount,
            TrelloBoardUrl = "https://trello.com/b/gYiilU64/daniel-mackay-ssw-backlog",
        };
    }

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

    private EmailViewModel GetEmail()
    {
        return new EmailViewModel
        {
            Subject = "Daniel Mackay - Daily Scrum",
            To = new EmailParticipantViewModel
            {
                Name = "SSWBenchMasters",
                Email = "SSWBenchMasters@ssw.com.au"
            },
            Cc =
            [
                new EmailParticipantViewModel
                {
                    Name = "DailyScrum",
                    Email = "SSWDailyScrum@ssw.com.au"
                }
            ]
        };
    }

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

    private DateOnly GetToday() => DateOnly.FromDateTime(DateTime.Now);

    private DateOnly GetLastWorkingDay()
    {
        var today = GetToday();
        var weekDay = today.DayOfWeek;

        return weekDay switch
        {
            DayOfWeek.Monday => today.AddDays(-3),
            DayOfWeek.Sunday => today.AddDays(-2),
            _ => today.AddDays(-1)
        };
    }
}
