using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Features.Identity;

namespace WebUI.Pages.Identity;

public class Index : PageModel
{
    private readonly ISender _sender;

    [BindProperty]
    public string AccessToken { get; set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public Index(ISender sender)
    {
        _sender = sender;
    }

    public void OnGet()
    {
    }

    public async Task OnPost()
    {
        var cmd = new UpdateAccessTokenCommand(AccessToken);
        await _sender.Send(cmd);

        Message = "Access Token Updated";
    }
}
