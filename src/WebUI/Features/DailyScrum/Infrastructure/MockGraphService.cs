using WebUI.Features.DailyScrum.Domain;

namespace WebUI.Features.DailyScrum.Infrastructure;

public class MockGraphService : IGraphService
{
    public Task<List<Project>> GetTasks(DateTime utcStart, DateTime utcEnd)
    {
        return Task.FromResult(new List<Project>());
    }

    public Task<int> GetInboxCount()
    {
        return Task.FromResult(99);
    }

    public void UpdateAccessToken(string accessToken)
    {
    }
}
