using ErrorOr;
using MediatR;
using WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand;

namespace WebUI.Features.DailyScrum.UseCases.RemoveTasksCommand;

public record RemoveTasksCommand(IEnumerable<Guid> YesterdaysTasks, IEnumerable<Guid> TodaysTasks) : IRequest<ErrorOr<Success>>;

public class RemoveTasksCommandHandler : IRequestHandler<RemoveTasksCommand, ErrorOr<Success>>
{
    private readonly IDailyScrumRepository _dailyScrumRepository;

    public RemoveTasksCommandHandler(IDailyScrumRepository dailyScrumRepository)
    {
        _dailyScrumRepository = dailyScrumRepository;
    }

    public Task<ErrorOr<Success>> Handle(RemoveTasksCommand request, CancellationToken cancellationToken)
    {
        var dailyScrum = _dailyScrumRepository.Get();
        if (dailyScrum == null)
            return Task.FromResult<ErrorOr<Success>>(Error.NotFound());

        dailyScrum.YesterdaysProjects.RemoveTasks(request.YesterdaysTasks);
        dailyScrum.TodaysProjects.RemoveTasks(request.YesterdaysTasks);

        _dailyScrumRepository.Save(dailyScrum);

        return Task.FromResult(new ErrorOr<Success>());
    }
}
