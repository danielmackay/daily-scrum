namespace WebUI.Features.DailyScrum.Domain;

public class ProjectList
{
    private readonly List<Project> _projects;
    public IReadOnlyList<Project> Projects => _projects.AsReadOnly();

    public ProjectList(IEnumerable<Project> projects)
    {
        _projects = projects.ToList();
    }

    public void RemoveTasks(IEnumerable<Guid> tasks)
    {
        foreach (var project in Projects)
        {
            project.RemoveTasks(tasks);
        }
    }
}
