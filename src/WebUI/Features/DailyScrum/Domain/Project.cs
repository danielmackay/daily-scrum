namespace WebUI.Features.DailyScrum.Domain;

public class Project
{
    private readonly List<TaskItem> _tasks = [];
    public string Name { get; }
    public bool IsSystemProject { get; }

    public IReadOnlyList<TaskItem> Tasks => _tasks;

    public Project(string name, bool isSystemProject, IEnumerable<TaskItem> tasks)
    {
        Name = RemoveEmojis(name);
        IsSystemProject = isSystemProject;
        _tasks.AddRange(tasks);
    }

    private static string RemoveEmojis(string input) => input.Replace("✅", "").TrimStart();

    public void RemoveTasks(IEnumerable<Guid> tasks)
    {
        _tasks.RemoveAll(t => tasks.Contains(t.Id));
    }
}
