using WebUI.Features.DailyScrum.Queries;

namespace WebUI.Common.ViewModels;

public class ProjectViewModel
{
    public string Name { get; init; }
    public bool IsSystemProject { get; init; }
    public IEnumerable<TaskViewModel> Tasks { get; init; }
}
