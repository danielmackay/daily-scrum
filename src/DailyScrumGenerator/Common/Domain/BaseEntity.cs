using MediatR;

namespace DailyScrumGenerator.Common.Domain;

public abstract class BaseEntity
{
    public readonly List<INotification> StagedEvents = [];
}