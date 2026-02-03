namespace Domain;

// TODO: This could be a DTO
public class TasksByDay
{
    // DM: This returns the correct date, but the day is always one off.
    public DateOnly Date { get; }

    public string DayOfWeek => Date.DayOfWeek.ToString();

    public List<Project> Projects { get; }

    public TasksByDay(DateOnly date, List<Project> projects)
    {
        Date = date;
        Projects = projects;
    }
}
