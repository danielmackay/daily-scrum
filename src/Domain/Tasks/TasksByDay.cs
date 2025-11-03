namespace Domain;

// TODO: This could be a DTO
public class TasksByDay
{
    public DateOnly Date { get; }
    public List<Project> Projects { get; }

    public TasksByDay(DateOnly date, List<Project> projects)
    {
        Date = date;
        Projects = projects;
    }
}
