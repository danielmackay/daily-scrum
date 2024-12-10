using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand;

public interface IDailyScrumRepository
{
    void Save(Domain.DailyScrum dailyScrum);

    Domain.DailyScrum? Get();
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
        // NOTE: Using newtonsoft json serializer because System.Text.Json had problems with deserializing
        var dailyScrumJson = Newtonsoft.Json.JsonConvert.SerializeObject(dailyScrum);
        _session.SetString(SessionKey, dailyScrumJson);
    }

    public Domain.DailyScrum? Get()
    {
        var dailyScrumJson = _session.GetString(SessionKey);
        if (string.IsNullOrWhiteSpace(dailyScrumJson))
            return null;

        return Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.DailyScrum>(dailyScrumJson);
    }
}
