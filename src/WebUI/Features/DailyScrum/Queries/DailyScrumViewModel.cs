namespace WebUI.Features.DailyScrum.Queries;

public class DailyScrumViewModel
{
    public required UserSummaryViewModel UserSummary { get; init; }
    public List<ProjectViewModel> YesterdaysProjects { get; init; } = [];
    public List<TaskViewModel> YesterdaysTasks { get; init; } = [];
    public List<ProjectViewModel> TodaysProjects { get; init; } = [];
    public List<TaskViewModel> TodaysTasks { get; init; } = [];
}

public class UserSummaryViewModel
{
    public int DaysUntilNextBooking { get; init; }
    public int InboxCount { get; init; }
    public required string TrelloBoardUrl { get; init; }
}

public class TaskViewModel
{
    public TaskStatus Status { get; init; }
    public required string Name { get; init; }
}

public class ProjectViewModel
{
    public string Name { get; init; } = null!;
    public List<TaskViewModel> Tasks { get; init; } = [];
}

public enum TaskStatus
{
    Todo = 1,
    InProgress = 2,
    Done = 3,
    Blocked = 4
}
