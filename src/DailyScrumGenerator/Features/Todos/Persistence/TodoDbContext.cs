using DailyScrumGenerator.Features.Todos.Domain;

// Disable as we want the partial class to be in the same namespace as the original class
// ReSharper disable once CheckNamespace
namespace DailyScrumGenerator.Common.Persistence;

public partial class AppDbContext
{
    public DbSet<Todo> Todos { get; set; } = null!;
}