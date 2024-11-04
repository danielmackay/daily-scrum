namespace WebUI.Common.Identity;

public interface ICurrentUserService
{
    string UserId { get; }

    string AccessToken { get; }
    string FirstName { get; }
    string LastName { get; }

    void UpdateAccessToken(string token);
    void UpdateName(string firstName, string lastName);
}

public class CurrentUserService : ICurrentUserService
{
    public string UserId { get; } = Guid.NewGuid().ToString();

    public string AccessToken { get; private set; } = string.Empty;

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public void UpdateAccessToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        AccessToken = token;
    }

    public void UpdateName(string firstName, string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        FirstName = firstName;
        LastName = lastName;
    }
}
