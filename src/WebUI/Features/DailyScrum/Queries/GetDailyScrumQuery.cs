using MediatR;
using WebUI.Common.Services;
using WebUI.Common.ViewModels;
using WebUI.Features.DailyScrum.Infrastructure;

namespace WebUI.Features.DailyScrum.Queries;

public record GetDailyScrumQuery(string Name, int? ClientDays, DateOnly? LastWorkingDay)
    : IRequest<DailyScrumViewModel>;

public class GetDailyScrumQueryHandler : IRequestHandler<GetDailyScrumQuery, DailyScrumViewModel>
{
    private readonly IGraphService _graphService;
    private readonly TimeProvider _timeProvider;

    public GetDailyScrumQueryHandler(IGraphService graphService, TimeProvider timeProvider)
    {
        _graphService = graphService;
        _timeProvider = timeProvider;
    }

    public async Task<DailyScrumViewModel> Handle(GetDailyScrumQuery request, CancellationToken cancellationToken)
    {
        var email = GetEmail(request.Name);

        var userSummary = await GetUserSummary(request.ClientDays);

        var today = _timeProvider.GetToday();
        var todaysProjects = await GetProjects(today);

        var yesterday = GetLastWorkingDay(request.LastWorkingDay);
        var yesterdaysProjects = await GetProjects(yesterday);

        return new DailyScrumViewModel
        {
            UserSummary = userSummary,
            TodaysProjects = todaysProjects,
            YesterdaysProjects = yesterdaysProjects,
            Email = email
        };
    }

    private async Task<UserSummaryViewModel> GetUserSummary(int? clientDays)
    {
        var inboxCount = await _graphService.GetInboxCount();

        return new UserSummaryViewModel
        {
            DaysUntilNextBooking = clientDays is null ? "♾️" : clientDays.Value.ToString(),
            InboxCount = inboxCount,
            TrelloBoardUrl = "https://trello.com/b/gYiilU64/daniel-mackay-ssw-backlog",
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

    private EmailViewModel GetEmail(string name)
    {
        return new EmailViewModel
        {
            Subject = $"{name} - Daily Scrum",
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

    private DateOnly GetLastWorkingDay(DateOnly? lastWorkingDay) =>
        lastWorkingDay ?? _timeProvider.GetToday().AddDays(-1);
}
