// using MediatR;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand;
//
// namespace WebUI.Pages.DailyScrum;
//
// public class Preview : PageModel
// {
//     private readonly ISender _sender;
//
//     [FromQuery]
//     public int? ClientDays { get; set; }
//
//     [FromQuery]
//     public DateOnly LastWorkingDay { get; set; }
//
//     public DailyScrumViewModel ViewModel { get; set; } = null!;
//
//     public Preview(ISender sender)
//     {
//         _sender = sender;
//     }
//
//     public async Task OnGet()
//     {
//         // TODO: Create query (or re-use existing) to pull daily scrum data
//         var query = new CreateDailyScrumCommand(ClientDays, LastWorkingDay);
//         // ViewModel = await _sender.Send(query);
//     }
// }
