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

    [BindProperty] public int? ClientDays { get; set; }

    [BindProperty] public DateOnly LastWorkingDay { get; set; }

    public Index(TimeProvider timeProvider, ISender sender)
    {
        _timeProvider = timeProvider;
    }

    public void OnGet()
    {
        LastWorkingDay = _timeProvider.GetToday().AddDays(-1);
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("/DailyScrum/EditTasks",
            new { ClientDays, LastWorkingDay = LastWorkingDay.ToString("O") });
    }
}
