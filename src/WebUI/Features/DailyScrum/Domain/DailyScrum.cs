namespace WebUI.Features.DailyScrum.Domain;

// TODO: Should this be a view model?
public class DailyScrum
{
    public UserSummary UserSummary { get; }
    public ProjectList YesterdaysProjects { get; }
    public ProjectList TodaysProjects { get; }
    public EmailSummary Email { get; }

    public DailyScrum(UserSummary userSummary, ProjectList yesterdaysProjects, ProjectList todaysProjects, EmailSummary email)
    {
        UserSummary = userSummary;
        YesterdaysProjects = yesterdaysProjects;
        TodaysProjects = todaysProjects;
        Email = email;
    }
}

// public class DailyScrum2
// {
//     public UserSummary UserSummary { get; }
//     public ProjectList YesterdaysProjects { get; }
//     public ProjectList TodaysProjects { get; }
//     public EmailSummary Email { get; }
//
//     public DailyScrum2(UserSummary userSummary, ProjectList yesterdaysProjects, ProjectList todaysProjects, EmailSummary email)
//     {
//         UserSummary = userSummary;
//         YesterdaysProjects = yesterdaysProjects;
//         TodaysProjects = todaysProjects;
//         Email = email;
//     }
// }
