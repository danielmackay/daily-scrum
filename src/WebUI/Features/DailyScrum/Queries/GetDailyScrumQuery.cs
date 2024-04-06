using MediatR;
using WebUI.Features.DailyScrum.Infrastructure;

namespace WebUI.Features.DailyScrum.Queries;

public class GetDailyScrumQuery : IRequest<DailyScrumViewModel>;

public class GetDailyScrumQueryHandler : IRequestHandler<GetDailyScrumQuery, DailyScrumViewModel>
{
    private readonly GraphService _graphService;

    public GetDailyScrumQueryHandler(GraphService graphService)
    {
        _graphService = graphService;
    }

    public async Task<DailyScrumViewModel> Handle(GetDailyScrumQuery request, CancellationToken cancellationToken)
    {
        var userSummary = new UserSummaryViewModel
        {
            DaysUntilNextBooking = 99,
            InboxCount = 72,
            TrelloBoardUrl = "https://trello.com/b/yourboard",
        };

        var today = GetToday();
        var todaysTasks = await GetTasks(today);

        var yesterday = GetLastWorkingDay();
        var yesterdaysTasks = await GetTasks(yesterday);

        return new DailyScrumViewModel
        {
            UserSummary = userSummary,
            TodaysTasks = todaysTasks,
            YesterdaysTasks = yesterdaysTasks,
        };
    }

    private async Task<List<TaskViewModel>> GetTasks(DateOnly date)
    {
        var (startOfDayUtc, endOfDayUtc) = GetTimeStamps(date);

        var graphTasks = await _graphService.GetTasks(startOfDayUtc, endOfDayUtc);

        var todaysTasks = graphTasks
            .Select(t => new TaskViewModel
            {
                Name = t.Title,
                Status = t.Status switch
                {
                    Microsoft.Graph.Models.TaskStatus.Completed => TaskStatus.Done,
                    _ => TaskStatus.Todo
                }
            })
            .ToList();

        return todaysTasks;
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
