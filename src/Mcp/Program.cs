using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

// Add MCP server services
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Map MCP server endpoints
app.MapMcp();

app.Run();

[McpServerToolType]
public static class HelloWorldTool
{
    [McpServerTool, Description("A simple greeting tool that says hello to the specified name")]
    public static string SayHello([Description("The name of the person to greet")] string name)
    {
        return $"Hello, {name}! Welcome to the MCP server.";
    }
}