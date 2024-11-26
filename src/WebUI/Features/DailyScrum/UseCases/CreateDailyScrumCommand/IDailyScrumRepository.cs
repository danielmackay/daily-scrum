using System.Text.Json;

namespace WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand;

public interface IDailyScrumRepository
{
    void Save(Domain.DailyScrum dailyScrum);
}


public class SessionDailyScrumRepository : IDailyScrumRepository
{
    private readonly ISession _session;
    private const string SessionKey = "DailyScrum";

    public SessionDailyScrumRepository(IHttpContextAccessor httpContextAccessor)
    {
        var session = httpContextAccessor.HttpContext?.Session;
        ArgumentNullException.ThrowIfNull(session);
        _session = session;
    }

    public void Save(Domain.DailyScrum dailyScrum)
    {
        var dailyScrumJson = JsonSerializer.Serialize(dailyScrum);
        _session.SetString(SessionKey, dailyScrumJson);
    }

    public Domain.DailyScrum? Get()
    {
        var dailyScrumJson = _session.GetString(SessionKey);
        return dailyScrumJson == null ? null : JsonSerializer.Deserialize<Domain.DailyScrum>(dailyScrumJson);
    }
}
