using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Features.DailyScrum.Queries;

namespace WebUI.Pages.DailyScrum;

public class Preview : PageModel
{
    private readonly ISender _sender;

    [BindProperty]
    public int? ClientDays { get; set; }

    [BindProperty]
    public DateOnly LastWorkingDay { get; set; }

    public DailyScrumViewModel ViewModel { get; set; } = null!;

    public Preview(ISender sender)
    {
        _sender = sender;
    }

    public async Task OnGet()
    {
        var query = new GetDailyScrumQuery(ClientDays, LastWorkingDay);
        ViewModel = await _sender.Send(query);
    }
}
