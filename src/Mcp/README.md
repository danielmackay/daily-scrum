# MCP Server - Hello World

This project implements a Model Context Protocol (MCP) server with a basic "Hello World" function using ASP.NET Core.

## What is MCP?

The Model Context Protocol (MCP) is an open protocol that enables seamless integration between AI assistants and external data sources and tools. MCP servers expose resources, prompts, and tools that AI models can interact with.

For more information about MCP:
- [Official Documentation](https://modelcontextprotocol.io/)
- [Protocol Specification](https://spec.modelcontextprotocol.io/)
- [GitHub Organization](https://github.com/modelcontextprotocol)

## Features

This MCP server provides:

- **SayHello** - A simple greeting tool that says hello to a specified name

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later

### Running the Server

1. Navigate to the Mcp project directory:
   ```bash
   cd src/Mcp
   ```

2. Run the server:
   ```bash
   dotnet run
   ```

The server will start and listen for MCP connections on the configured ports (check console output for the actual URLs, typically `http://localhost:5000` or `https://localhost:5001`).

## Using the MCP Server

### Connecting from an MCP Client

MCP clients can connect to this server using HTTP transport. The server exposes its capabilities through the MCP endpoints at the configured base URL.

### Available Tools

#### SayHello

Greets a person by name.

**Parameters:**
- `name` (string, required) - The name of the person to greet

**Example Usage:**
When called with the name "Alice", the tool returns: `"Hello, Alice! Welcome to the MCP server."`

## Configuration for MCP Clients

### Claude Desktop Configuration

Add the following to your Claude Desktop MCP configuration file:

**Windows:** `%APPDATA%\Claude\claude_desktop_config.json`
**macOS:** `~/Library/Application Support/Claude/claude_desktop_config.json`

```json
{
  "mcpServers": {
    "hello-world": {
      "command": "dotnet",
      "args": ["run", "--project", "C:/path/to/src/Mcp/Mcp.csproj"],
      "env": {}
    }
  }
}
```

Replace `C:/path/to/` with the actual path to your project (use forward slashes even on Windows).

### Using with HTTP Client

Since this is an HTTP-based MCP server, you can also configure clients to connect via HTTP:

```json
{
  "mcpServers": {
    "hello-world-http": {
      "url": "http://localhost:5000"
    }
  }
}
```

Make sure the server is running before the client attempts to connect.

## Development

### Adding New Tools

To add new tools to the MCP server:

1. Create a static class and mark it with `[McpServerToolType]`
2. Add static methods marked with `[McpServerTool]` and a `[Description]` attribute
3. Use `[Description]` attributes on parameters to document them

Example:

```csharp
[McpServerToolType]
public static class MyTools
{
    [McpServerTool, Description("Adds two numbers together")]
    public static int Add(
        [Description("The first number")] int a,
        [Description("The second number")] int b)
    {
        return a + b;
    }
}
```

The tool will be automatically discovered and registered when the server starts.

### Project Structure

- `Program.cs` - Main server configuration and tool definitions
- `Mcp.csproj` - Project file with MCP dependencies
- `appsettings.json` - Application configuration
- `README.md` - This documentation

## Dependencies

- `ModelContextProtocol.AspNetCore` (v0.4.0-preview.3) - MCP server framework for ASP.NET Core
- `Microsoft.AspNetCore.OpenApi` (v9.0.10) - OpenAPI support

## Building

Build the project using:

```bash
dotnet build
```

## Testing

You can test the MCP server by:

1. Running it with `dotnet run`
2. Connecting with an MCP-compatible client (like Claude Desktop)
3. Invoking the `SayHello` tool through the client

## License

See the LICENSE file in the root of the repository.

