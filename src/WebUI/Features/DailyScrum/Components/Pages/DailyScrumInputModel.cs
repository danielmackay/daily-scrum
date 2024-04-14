namespace WebUI.Features.DailyScrum.Components.Pages;

public class DailyScrumInputModel
{
    public string Name { get; set; } = String.Empty;

    public int? ClientDays { get; set; }

    public DateOnly LastWorkingDay { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
}
