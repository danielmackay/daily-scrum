using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Common.Services;
using WebUI.Features.DailyScrum.Queries;

namespace WebUI.Pages.DailyScrum;

public class Index : PageModel
{
    private readonly TimeProvider _timeProvider;

    // NOTE: THis needs to be a string due to the following bug with nullable fields
    // https://github.com/dotnet/aspnetcore/issues/52195
    [BindProperty]
    public int? ClientDays { get; set; }

    [BindProperty]
    public DateOnly LastWorkingDay { get; set; }

    public Index(TimeProvider timeProvider, ISender sender)
    {
        _timeProvider = timeProvider;
    }

    public void OnGet()
    {
        if (LastWorkingDay == default)
            LastWorkingDay = _timeProvider.GetToday().AddDays(-1);
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("/DailyScrum/Preview", new { ClientDays, LastWorkingDay = LastWorkingDay.ToString("O")});

        // TODO: Redirect to preview page
        // var url = $"/daily-scrum/preview?clientDays={ClientDays}&lastWorkingDay={LastWorkingDay.ToString("O")}";

    }
}
