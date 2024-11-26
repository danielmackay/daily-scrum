namespace WebUI.Features.DailyScrum.Domain;

public class EmailSummary
{
    public string Subject { get; }
    public EmailParticipant To { get; }
    public IReadOnlyList<EmailParticipant> Cc { get; }

    public EmailSummary(string userName)
    {
        Subject = $"{userName} - Daily Scrum";
        To = new EmailParticipant
        {
            Name = "SSWBenchMasters",
            Email = "SSWBenchMasters@ssw.com.au"
        };
        Cc =
        [
            new EmailParticipant
            {
                Name = "DailyScrum",
                Email = "SSWDailyScrum@ssw.com.au"
            }
        ];
    }
}
