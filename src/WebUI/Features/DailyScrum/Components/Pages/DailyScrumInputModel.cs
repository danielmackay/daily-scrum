using System.ComponentModel.DataAnnotations;

namespace WebUI.Features.DailyScrum.Components.Pages;

public class DailyScrumInputModel
{
    [Required]
    public string Name { get; set; } = String.Empty;

    // NOTE: THis needs to be a string due to the following bug with nullable fields
    // https://github.com/dotnet/aspnetcore/issues/52195
    public string ClientDays { get; set; } = String.Empty;

    public DateOnly LastWorkingDay { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
}
