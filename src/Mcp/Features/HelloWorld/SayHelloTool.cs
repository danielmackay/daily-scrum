using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Mcp.Features.HelloWorld;

[McpServerToolType]
public static class SayHelloTool
{
    [McpServerTool, Description("A simple greeting tool that says hello to the specified name")]
    public static string SayHello([Description("The name of the person to greet")] string name)
    {
        return $"Hello, {name}! Welcome to the MCP server.";
    }
}
