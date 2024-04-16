using WebUI.Common.ViewModels;
using WebUI.Features.DailyScrum.Queries;

namespace WebUI.Features.Timesheet.Queries;

public class TimesheetViewModel
{
    public List<ProjectViewModel> Projects { get; init; } = [];
}
