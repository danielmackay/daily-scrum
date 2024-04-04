using MediatR;

namespace DailyScrumGenerator.Features.Todos.Domain.Events;

public record TodoCreatedEvent(Guid TodoId) : INotification;