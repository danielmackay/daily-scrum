using ErrorOr;
using MediatR;
using WebUI.Features.DailyScrumEmail.UseCases.CreateDailyScrumCommand;

namespace WebUI.Features.DailyScrumEmail.UseCases.GetDailyScrumQuery;

public record GetDailyScrumQuery : IRequest<ErrorOr<Domain.DailyScrum>>;

public class GetDailyScrumQueryHandler : IRequestHandler<GetDailyScrumQuery, ErrorOr<Domain.DailyScrum>>
{
    private readonly IDailyScrumRepository _dailyScrumRepository;

    public GetDailyScrumQueryHandler(IDailyScrumRepository dailyScrumRepository)
    {
        _dailyScrumRepository = dailyScrumRepository;
    }

    public Task<ErrorOr<Domain.DailyScrum>> Handle(GetDailyScrumQuery request, CancellationToken cancellationToken)
    {
        var dailyScrum = _dailyScrumRepository.Get();
        if (dailyScrum == null)
        {
            return Task.FromResult<ErrorOr<Domain.DailyScrum>>(Error.NotFound());
        }

        return Task.FromResult<ErrorOr<Domain.DailyScrum>>(dailyScrum);
    }
}
