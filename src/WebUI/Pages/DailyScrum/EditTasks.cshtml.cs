using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Common.ViewModels;
using WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand;
using WebUI.Features.DailyScrum.UseCases.GetDailyScrumQuery;

namespace WebUI.Pages.DailyScrum;

public class EditTasks : PageModel
{
    private readonly ISender _sender;

    [BindProperty]
    public List<EditProjectViewModel> YesterdaysProjects { get; set; } = [];

    public EditTasks(ISender sender)
    {
        _sender = sender;
    }

    public async Task OnGet()
    {
        var query = new GetDailyScrumQuery();
        var result = await _sender.Send(query);

        // TODO: Handle error

        YesterdaysProjects = result.YesterdaysProjects.Select(p =>
            new EditProjectViewModel
            {
                Name = p.Name,
                Tasks = p.Tasks.Select(t => new EditTaskViewModel
                {
                    Id = Guid.NewGuid(),
                    Include = true,
                    Name = t.Name
                }).ToList()
            }).ToList();
    }

    public void OnPost()
    {
        // TODO: Create command to update daily scrum data
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
