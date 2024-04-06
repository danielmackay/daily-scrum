namespace WebUI.Common;

public interface ICurrentUserService
{
    public string? UserId { get; }
}

public class CurrentUserService : ICurrentUserService
{
    public string? UserId { get; } = Guid.NewGuid().ToString();
}
