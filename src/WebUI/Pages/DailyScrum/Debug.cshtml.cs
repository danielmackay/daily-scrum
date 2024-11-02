using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.DailyScrum;

public class Debug : PageModel
{
    private readonly TimeProvider _timeProvider;

    public DateTimeOffset Now { get; private set; }

    public Debug(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public void OnGet()
    {
        Now = _timeProvider.GetLocalNow();
    }
}
