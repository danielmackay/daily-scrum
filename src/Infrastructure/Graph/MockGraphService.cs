using Domain;
using TaskStatus = Domain.TaskStatus;

namespace Infrastructure.Graph;

public class MockGraphService : IGraphService
{
    public Task<List<Project>> GetTasks(DateTime utcStart, DateTime utcEnd)
    {
        var projects = new List<Project>
        {
            new Project("Project 1", false, new List<TaskItem>
            {
                new TaskItem(TaskStatus.Done, "Task A"),
                new TaskItem(TaskStatus.InProgress, "Task B"),
                new TaskItem(TaskStatus.Todo, "Task C"),
            }),
            new Project("Project 2", false, new List<TaskItem>
            {
                new TaskItem(TaskStatus.Done, "Task A"),
                new TaskItem(TaskStatus.InProgress, "Task B"),
                new TaskItem(TaskStatus.Todo, "Task C"),
            }),
            new Project("Project 3", false, new List<TaskItem>
            {
                new TaskItem(TaskStatus.Done, "Task A"),
                new TaskItem(TaskStatus.InProgress, "Task B"),
                new TaskItem(TaskStatus.Todo, "Task C"),
            }),
        };

        return Task.FromResult(projects);
    }

    public Task<int> GetInboxCount()
    {
        return Task.FromResult(99);
    }

    public Task<List<TasksByDay>> GetTasksByDay(DateTime utcStart, DateTime utcEnd)
    {
        var tasksByDay = new List<TasksByDay>
        {
            new TasksByDay(utcStart.Date, new List<Project>
            {
                new Project("Project 1", false, new List<TaskItem>
                {
                    new TaskItem(TaskStatus.Done, "Task A"),
                    new TaskItem(TaskStatus.InProgress, "Task B"),
                    new TaskItem(TaskStatus.Todo, "Task C"),
                }),
                new Project("Project 2", false, new List<TaskItem>
                {
                    new TaskItem(TaskStatus.Done, "Task D"),
                    new TaskItem(TaskStatus.InProgress, "Task E"),
                }),
            }),
            new TasksByDay(utcStart.Date.AddDays(1), new List<Project>
            {
                new Project("Project 3", false, new List<TaskItem>
                {
                    new TaskItem(TaskStatus.Done, "Task F"),
                    new TaskItem(TaskStatus.Todo, "Task G"),
                }),
            }),
        };

        return Task.FromResult(tasksByDay);
    }
}
