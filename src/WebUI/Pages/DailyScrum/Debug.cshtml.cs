using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.DailyScrum;

public class Debug : PageModel
{
    public DateTimeOffset Now { get; private set; }

    public Debug(TimeProvider timeProvider)
    {
        Now = timeProvider.GetLocalNow();
    }

    public void OnGet()
    {

    }
}
