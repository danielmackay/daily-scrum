namespace Domain;

public class ProjectList
{
    private readonly List<Project> _projects = [];

    public IReadOnlyList<Project> Projects => _projects;

    public ProjectList(IEnumerable<Project> projects)
    {
        _projects.AddRange(projects);
    }

    public void RemoveTasks(IEnumerable<Guid> tasks)
    {
        foreach (var project in Projects)
        {
            project.RemoveTasks(tasks);
        }
    }
}
