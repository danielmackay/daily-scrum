namespace Domain;

public class TasksByDay
{
    public DateTime Date { get; }
    public List<Project> Projects { get; }

    public TasksByDay(DateTime date, List<Project> projects)
    {
        Date = date.Date;
        Projects = projects;
    }
}
