namespace WebUI.Features.DailyScrum;

public class DailyScrumModel
{
    public int DaysUntilNextBooking { get; init; }
    public int InboxCount { get; init; }
    public required string TrelloBoardUrl { get; init; }
    public List<ProjectModel> YesterdaysProjects { get; init; } = new();
    public List<TaskModel> YesterdaysTasks { get; init; } = new();
    public List<ProjectModel> TodaysProjects { get; init; } = new();
    public List<TaskModel> TodaysTasks { get; init; } = new();
}

public class TaskModel
{
    public TaskStatus Status { get; init; }
    public required string Name { get; init; }
}

public class ProjectModel
{
    public string Name { get; init; } = null!;
    public List<TaskModel> Tasks { get; init; } = new();
}

public enum TaskStatus
{
    Todo = 1,
    InProgress = 2,
    Done = 3,
    Blocked = 4
}
