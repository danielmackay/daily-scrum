namespace WebUI.Features.DailyScrum.Domain;

public class UserSummary
{
    public int DaysUntilNextBooking { get; init; }
    public string DaysUntilNextBookingText { get; init; }
    public int InboxCount { get; init; }
    public string TrelloBoardUrl { get; init; }

    public UserSummary(int? daysUntilNextBooking, int inboxCount)
    {
        DaysUntilNextBooking = daysUntilNextBooking ?? 0;
        DaysUntilNextBookingText = daysUntilNextBooking is null ? "♾️" : daysUntilNextBooking.Value.ToString();
        InboxCount = inboxCount;
        TrelloBoardUrl = "https://trello.com/b/gYiilU64/daniel-mackay-ssw-backlog";
    }
}
