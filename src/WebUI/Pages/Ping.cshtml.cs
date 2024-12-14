using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages;

[AllowAnonymous]
public class Ping : PageModel
{
    public void OnGet()
    {

    }
}
