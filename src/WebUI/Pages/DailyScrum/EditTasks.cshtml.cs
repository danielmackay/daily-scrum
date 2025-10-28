using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Features.DailyScrumEmail.UseCases.GetDailyScrumQuery;
using WebUI.Features.DailyScrumEmail.UseCases.RemoveTasksCommand;
// using WebUI.Common.ViewModels;

namespace WebUI.Pages.DailyScrum;

public class EditTasks : PageModel
{
    private readonly ISender _sender;

    [BindProperty]
    public List<EditProjectViewModel> YesterdaysProjects { get; set; } = [];

    [BindProperty]
    public List<EditProjectViewModel> TodaysProjects { get; set; } = [];

    public EditTasks(ISender sender)
    {
        _sender = sender;
    }

    public async Task OnGet()
    {
        var query = new GetDailyScrumQuery();
        var result = await _sender.Send(query);

        // TODO: Handle error

        YesterdaysProjects = result.Value.YesterdaysProjects.Projects.Select(p =>
            new EditProjectViewModel
            {
                Name = p.Name,
                Tasks = p.Tasks.Select(t => new EditTaskViewModel
                {
                    Id = t.Id,
                    Include = true,
                    Name = t.Name
                }).ToList()
            }).ToList();

        TodaysProjects = result.Value.TodaysProjects.Projects.Select(p =>
            new EditProjectViewModel
            {
                Name = p.Name,
                Tasks = p.Tasks.Select(t => new EditTaskViewModel
                {
                    Id = t.Id,
                    Include = true,
                    Name = t.Name
                }).ToList()
            }).ToList();
    }

    public async Task<IActionResult> OnPost()
    {
        var yesterdaysTasksToRemove = YesterdaysProjects
            .SelectMany(p => p.Tasks)
            .Where(t => !t.Include)
            .Select(t => t.Id)
            .ToList();

        var todaysTasksToRemove = TodaysProjects
            .SelectMany(p => p.Tasks)
            .Where(t => !t.Include)
            .Select(t => t.Id)
            .ToList();

        var command = new RemoveTasksCommand(yesterdaysTasksToRemove, todaysTasksToRemove);
        var result = await _sender.Send(command);

        // TODO: Handle error

        return RedirectToPage("/DailyScrum/Email");
    }
}

public class EditTaskViewModel
{
    public required bool Include { get; init; }
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

public class EditProjectViewModel
{
    public required string Name { get; init; }
    public List<EditTaskViewModel> Tasks { get; init; } = [];
}
