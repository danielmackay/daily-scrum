using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace WebUI.Pages;

[AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    private readonly GraphServiceClient _graphServiceClient;

    public IndexModel(
        ILogger<IndexModel> logger,
        GraphServiceClient graphServiceClient)
    {
        _logger = logger;
        _graphServiceClient = graphServiceClient;
    }

    public async Task OnGet()
    {
        var user = await _graphServiceClient.Me.GetAsync();

        ViewData["ApiResult"] = user?.DisplayName;
    }
}