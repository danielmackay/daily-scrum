namespace WebUI.Features.DailyScrum.Domain;

public class UserSummary
{
    public int DaysUntilNextBooking { get; }
    public string DaysUntilNextBookingText { get; }
    public int InboxCount { get; }
    public string TrelloBoardUrl { get; }

    public UserSummary(int? daysUntilNextBooking, int inboxCount)
    {
        DaysUntilNextBooking = daysUntilNextBooking ?? 0;
        DaysUntilNextBookingText = daysUntilNextBooking is null ? "♾️" : daysUntilNextBooking.Value.ToString();
        InboxCount = inboxCount;
        TrelloBoardUrl = "https://trello.com/b/gYiilU64/daniel-mackay-ssw-backlog";
    }
}

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
