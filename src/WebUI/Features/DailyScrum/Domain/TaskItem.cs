namespace WebUI.Features.DailyScrum.Domain;

public class TaskItem
{
    private readonly List<string> _blockedEmojis = ["❌", "🚫", "⛔"];

    public TaskStatus Status { get; }
    public string Name { get; }
    // public string RawName { get; }

    public TaskItem(TaskStatus status, string name)
    {
        // RawName = name;
        Status = OverrideStatus(ref name, status);
        Name = $"{GetEmojis(Status)} {name}";
    }

    private TaskStatus OverrideStatus(ref string name, TaskStatus status)
    {
        foreach (var emoji in _blockedEmojis)
        {
            if (name.Contains(emoji))
            {
                name = "Blocked - " + name.Replace(emoji, "").TrimStart();
                return TaskStatus.Blocked;
            }
        }

        return status;
    }


    private string GetEmojis(Features.DailyScrum.Domain.TaskStatus status)
    {
        return status switch
        {
            Domain.TaskStatus.Done => "✅",
            Domain.TaskStatus.Blocked => "❌",
            _ => "⌛"
        };
    }
}
