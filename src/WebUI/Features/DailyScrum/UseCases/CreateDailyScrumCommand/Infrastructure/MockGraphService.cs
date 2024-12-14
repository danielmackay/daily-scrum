using WebUI.Features.DailyScrum.Domain;
using TaskStatus = WebUI.Features.DailyScrum.Domain.TaskStatus;

namespace WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand.Infrastructure;

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
            // new Project("Project 2", false, new List<TaskItem>
            // {
            //     new TaskItem(TaskStatus.Done, "Task A"),
            //     new TaskItem(TaskStatus.InProgress, "Task B"),
            //     new TaskItem(TaskStatus.Todo, "Task C"),
            // }),
            // new Project("Project 3", false, new List<TaskItem>
            // {
            //     new TaskItem(TaskStatus.Done, "Task A"),
            //     new TaskItem(TaskStatus.InProgress, "Task B"),
            //     new TaskItem(TaskStatus.Todo, "Task C"),
            // }),
        };

        return Task.FromResult(projects);
    }

    public Task<int> GetInboxCount()
    {
        return Task.FromResult(99);
    }
}
