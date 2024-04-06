namespace WebUI.Features.DailyScrum.Queries;

public class DailyScrumViewModel
{
    public required UserSummaryViewModel UserSummary { get; init; }
    public List<ProjectViewModel> YesterdaysProjects { get; init; } = [];
    //public List<TaskViewModel> YesterdaysTasks { get; init; } = [];
    public List<ProjectViewModel> TodaysProjects { get; init; } = [];
    //public List<TaskViewModel> TodaysTasks { get; init; } = [];
}

public class UserSummaryViewModel
{
    public int DaysUntilNextBooking { get; init; }
    public int InboxCount { get; init; }
    public required string TrelloBoardUrl { get; init; }
}

public class TaskViewModel
{
    public string Name { get; init; }
}

public class ProjectViewModel
{
    public string Name { get; init; }
    public bool IsSystemProject { get; init; }
    public IEnumerable<TaskViewModel> Tasks { get; init; }
}
