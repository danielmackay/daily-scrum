using MediatR;

namespace DailyScrumGenerator.Features.Todos.Domain.Events;

public record TodoCompletedEvent(Guid TodoId) : INotification;