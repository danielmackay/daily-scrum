namespace WebUI.Features.DailyScrum.Domain;

public class TaskItem
{
    private static readonly List<string> _blockedEmojis = ["❌", "🚫", "⛔"];

    public TaskStatus Status { get; }

    public string Name { get; }

    public Guid Id { get; }

    public TaskItem(TaskStatus status, string name, Guid? id = null)
    {
        Status = OverrideStatus(ref name, status);
        Name = $"{GetEmojis(Status)} {name}";
        Id = id ?? Guid.NewGuid();
    }

    // public static TaskItem Create(TaskStatus status, string name)
    // {
    //     var task = new TaskItem();
    //     task.Status = OverrideStatus(ref name, status);
    //     task.Name = $"{task.GetEmojis(task.Status)} {name}";
    //     task.Id = Guid.NewGuid();
    //     return task;
    // }

    private static TaskStatus OverrideStatus(ref string name, TaskStatus status)
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


    private string GetEmojis(TaskStatus status)
    {
        return status switch
        {
            TaskStatus.Done => "✅",
            TaskStatus.Blocked => "❌",
            _ => "⌛"
        };
    }
}
