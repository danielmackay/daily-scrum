using ErrorOr;
using MediatR;
using WebUI.Common.Identity;
using WebUI.Common.Services;
using WebUI.Features.DailyScrum.Domain;
using WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand.Infrastructure;

namespace WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand;

public record CreateDailyScrumCommand(int? ClientDays, DateOnly? LastWorkingDay)
    : IRequest<ErrorOr<Success>>;

public class CreateDailyScrumCommandHandler : IRequestHandler<CreateDailyScrumCommand, ErrorOr<Success>>
{
    private readonly IGraphService _graphService;
    private readonly TimeProvider _timeProvider;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDailyScrumRepository _dailyScrumRepository;
    private readonly ILogger<CreateDailyScrumCommandHandler> _logger;

    public CreateDailyScrumCommandHandler(
        IGraphService graphService,
        TimeProvider timeProvider,
        ICurrentUserService currentUserService,
        IDailyScrumRepository dailyScrumRepository,
        ILogger<CreateDailyScrumCommandHandler> logger)
    {
        _graphService = graphService;
        _timeProvider = timeProvider;
        _currentUserService = currentUserService;
        _dailyScrumRepository = dailyScrumRepository;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> Handle(CreateDailyScrumCommand request, CancellationToken cancellationToken)
    {
        var email = GetEmail();

        var userSummary = await GetUserSummary(request.ClientDays);

        var today = _timeProvider.GetToday();
        _logger.LogInformation("Getting projects for {Today}", today);

        var todaysProjects = await GetProjects(today);

        var yesterday = GetLastWorkingDay(request.LastWorkingDay);
        var yesterdaysProjects = await GetProjects(yesterday);

        var dailyScrum = new Domain.DailyScrum(userSummary, yesterdaysProjects, todaysProjects, email);

        _dailyScrumRepository.Save(dailyScrum);

        return new Success();
    }

    private async Task<UserSummary> GetUserSummary(int? clientDays)
    {
        var inboxCount = await _graphService.GetInboxCount();
        var userSummary = new UserSummary(clientDays, inboxCount);
        return userSummary;
    }

    private async Task<ProjectList> GetProjects(DateOnly date)
    {
        // TODO: This is not returning the correct timestamps on local vs docker
        var startOfDayUtc = _timeProvider.GetStartOfDayUtc(date);
        var endOfDayUtc = _timeProvider.GetEndOfDayUtc(date);

        _logger.LogInformation("Getting projects for {Date} ({StartOfDayUtc} to {EndOfDayUtc})", date, startOfDayUtc,
            endOfDayUtc);

        var projects = await _graphService.GetTasks(startOfDayUtc, endOfDayUtc);

        return new ProjectList(projects);
    }

    private EmailSummary GetEmail()
    {
        var userName = _currentUserService.UserName;
        var email = new EmailSummary(userName);
        return email;
    }

    private DateOnly GetLastWorkingDay(DateOnly? lastWorkingDay) =>
        lastWorkingDay ?? _timeProvider.GetToday().AddDays(-1);
}
