namespace WebUI.Features.DailyScrum.Domain;

// TODO: Should this be a view model?
public class DailyScrum
{
    public UserSummary UserSummary { get; }
    public ProjectList YesterdaysProjects { get; }
    public ProjectList TodaysProjects { get; }
    public EmailSummary Email { get; }

    public DailyScrum(UserSummary userSummary, IEnumerable<Project> yesterdaysProjects, IEnumerable<Project> todaysProjects, EmailSummary email)
    {
        UserSummary = userSummary;
        YesterdaysProjects = new ProjectList(yesterdaysProjects.ToList());
        TodaysProjects = new ProjectList(todaysProjects.ToList());
        Email = email;
    }
}
