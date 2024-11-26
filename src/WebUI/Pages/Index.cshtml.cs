using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand;

namespace WebUI.Pages;

// [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
public class IndexModel : PageModel
{
    // private readonly GraphServiceClient _graphServiceClient;

    public IndexModel(
        GraphServiceClientFactory factory)
    {
        // _graphServiceClient = factory.CreateWithAccessToken();
    }

    public async Task OnGet()
    {
        // var user = await _graphServiceClient.Me.GetAsync();
        //
        // ViewData["ApiResult"] = user?.DisplayName;
    }
}
