namespace WebUI.Features.DailyScrum.Domain;

// TODO: Should this be a view model?
public class DailyScrum
{
    public UserSummary UserSummary { get; }
    public List<Project> YesterdaysProjects { get; }
    public List<Project> TodaysProjects { get; }
    public EmailSummary Email { get; }

    public DailyScrum(UserSummary userSummary, IEnumerable<Project> yesterdaysProjects, IEnumerable<Project> todaysProjects, EmailSummary email)
    {
        UserSummary = userSummary;
        YesterdaysProjects = yesterdaysProjects.ToList();
        TodaysProjects = todaysProjects.ToList();
        Email = email;
    }
}
