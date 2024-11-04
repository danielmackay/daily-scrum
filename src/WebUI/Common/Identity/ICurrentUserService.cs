using Microsoft.Graph.Models;
using System.Security.Claims;

namespace WebUI.Common.Identity;

public interface ICurrentUserService
{
    string UserId { get; }

    string AccessToken { get; }
    string UserName { get; }

    void UpdateAccessToken(string token);
    void UpdateName(string firstName, string lastName);
}

public class CurrentUserService : ICurrentUserService
{
    public string UserId { get; } = Guid.NewGuid().ToString();

    public string AccessToken { get; private set; } = string.Empty;

    public string UserName { get; private set; } = string.Empty;

    public void UpdateAccessToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        AccessToken = token;
    }

    public void UpdateName(string firstName, string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        UserName = $"{firstName} {lastName}";
    }
}

public class OAuthCurrentUserService : ICurrentUserService
{
    public string UserId { get; }
    public string AccessToken { get; } = string.Empty;
    public string UserName { get; }

    public void UpdateAccessToken(string token) => throw new NotImplementedException();

    public void UpdateName(string firstName, string lastName) => throw new NotImplementedException();

    public OAuthCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        UserName = httpContextAccessor.HttpContext?.User?.FindFirstValue("Name") ?? string.Empty;
        UserName = UserName.TrimEnd(" [SSW]");
    }
}
