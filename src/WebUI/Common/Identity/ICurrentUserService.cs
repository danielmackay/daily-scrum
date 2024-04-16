namespace WebUI.Common.Identity;

public interface ICurrentUserService
{
    public string? UserId { get; }

    public string AccessToken { get; }

    public void UpdateAccessToken(string token);
}

public class CurrentUserService : ICurrentUserService
{
    public string? UserId { get; } = Guid.NewGuid().ToString();

    public string? AccessToken { get; private set; }

    public void UpdateAccessToken(string token)
    {
        AccessToken = token;
    }
}
