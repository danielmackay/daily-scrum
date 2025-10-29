namespace Mcp.Features.Tasks;

public class GetTasksResponse
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<TasksByDay> TasksByDay { get; set; } = new();
}

public class TasksByDay
{
    public DateTime Date { get; set; }
    public List<ProjectTasks> Projects { get; set; } = new();
}

public class ProjectTasks
{
    public string Name { get; set; } = string.Empty;
    public bool IsSystemProject { get; set; }
    public List<TaskDetails> Tasks { get; set; } = new();
}

public class TaskDetails
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class GetTasksErrorResponse
{
    public string Error { get; set; } = string.Empty;
}
