using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Common.Services;
using WebUI.Features.DailyScrumEmail.UseCases.CreateDailyScrumCommand;

namespace WebUI.Pages.DailyScrum;

public class Index : PageModel
{
    private readonly TimeProvider _timeProvider;
    private readonly ISender _sender;

    [BindProperty] public int? ClientDays { get; set; }

    [BindProperty] public DateOnly LastWorkingDay { get; set; }

    public Index(TimeProvider timeProvider, ISender sender)
    {
        _timeProvider = timeProvider;
        _sender = sender;
    }

    public void OnGet()
    {
        LastWorkingDay = _timeProvider.GetToday().AddDays(-1);
    }

    public async Task<IActionResult> OnPost()
    {
        var query = new CreateDailyScrumCommand(ClientDays, LastWorkingDay);
        // TODO: Handle error
        var result = await _sender.Send(query);

        return RedirectToPage("/DailyScrum/EditTasks");
    }
}
