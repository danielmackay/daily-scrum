using Infrastructure.Graph;
using Infrastructure.Identity;
using Mcp.Features.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Add logging
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Configure options
builder.Services.Configure<MsToDoOptions>(builder.Configuration.GetSection(MsToDoOptions.SectionName));

// Add services
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<GraphServiceClientFactory>();
builder.Services.AddScoped<IGraphService, GraphService>();

// Add MCP server services
builder.Services
    .AddMcpServer()
    // .WithHttpTransport()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Map MCP server endpoints
// app.MapMcp();

app.Run();
