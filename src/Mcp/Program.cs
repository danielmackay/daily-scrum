using Infrastructure.Graph;
using Infrastructure.Identity;
using ModelContextProtocol.Server;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.AddConsole();

// Add services
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<GraphServiceClientFactory>();
builder.Services.AddScoped<IGraphService, GraphService>();

// Add MCP server services
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Map MCP server endpoints
app.MapMcp();

app.Run();