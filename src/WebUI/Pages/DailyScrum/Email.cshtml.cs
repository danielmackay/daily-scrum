using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Features.DailyScrum.UseCases.GetDailyScrumQuery;

namespace WebUI.Pages.DailyScrum;

public class Email : PageModel
{
    private readonly ISender _sender;
    public Domain.DailyScrum ViewModel { get; set; } = null!;

    public Email(ISender sender)
    {
        _sender = sender;
    }

    public async Task OnGet()
    {
        var query = new GetDailyScrumQuery();
        var result = await _sender.Send(query);

        // TODO: Handle error

        ViewModel = result.Value;
    }

    public async Task OnPost()
    {
        // TODO: Send email

        // TODO: Redirect to success page
    }
}
