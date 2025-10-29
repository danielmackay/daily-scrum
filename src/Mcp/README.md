# Daily Scrum MCP Server

This project implements a Model Context Protocol (MCP) server that provides access to Microsoft To-Do tasks for daily scrum reporting.

The server uses STDIO transport to communicate with MCP clients like Claude Desktop.

## What is MCP?

The Model Context Protocol (MCP) is an open protocol that enables seamless integration between AI assistants and external data sources and tools. MCP servers expose resources, prompts, and tools that AI models can interact with.

For more information about MCP:
- [Official Documentation](https://modelcontextprotocol.io/)
- [Protocol Specification](https://spec.modelcontextprotocol.io/)
- [GitHub Organization](https://github.com/modelcontextprotocol)

## Features

This MCP server provides:

- **GetTasks** - Retrieves Microsoft To-Do tasks grouped by day and project/list
- **SetAccessToken** - Sets the Microsoft Graph access token for authentication (optional if using environment variable)

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- Microsoft Graph API access token (see Configuration section below)

### Getting a Microsoft Graph Access Token

1. Navigate to [Microsoft Graph Explorer](https://developer.microsoft.com/en-us/graph/graph-explorer)
2. Sign in with your Microsoft account
3. Grant the required permissions (Tasks.Read, Tasks.ReadWrite)
4. Copy your access token from the Graph Explorer

### Running the Server

The server is typically started automatically by the MCP client (e.g., Claude Desktop). You don't need to run it manually.

To test the server manually:

1. Navigate to the Mcp project directory:
   ```bash
   cd src/Mcp
   ```

2. Set your access token as an environment variable:
   ```bash
   # Linux/macOS
   export MSTODO__ACCESSTOKEN="your-access-token-here"
   
   # Windows PowerShell
   $env:MSTODO__ACCESSTOKEN="your-access-token-here"
   ```

3. Run the server:
   ```bash
   dotnet run
   ```

The server will start and communicate via STDIO (standard input/output).

## Using the MCP Server

The server is designed to be used through MCP clients like Claude Desktop. Once configured (see Configuration section), you can ask Claude to retrieve your tasks.

### Available Tools

#### GetTasks

Retrieves Microsoft To-Do tasks grouped by day and project/list.

**Parameters:**
- `utcStart` (DateTime, required) - Start date in ISO 8601 format (e.g., 2025-10-29T00:00:00Z)
- `utcEnd` (DateTime, required) - End date in ISO 8601 format (e.g., 2025-10-29T23:59:59Z)

**Returns:**
A JSON object containing tasks grouped by day, with each day containing projects and their associated tasks.

#### SetAccessToken

Sets the Microsoft Graph access token for authentication.

**Parameters:**
- `token` (string, required) - The Microsoft Graph access token

**Note:** If you configure the access token via environment variable (recommended), you don't need to use this tool.

## Configuration for MCP Clients

### Claude Desktop Configuration

Add the following to your Claude Desktop MCP configuration file:

**Windows:** `%APPDATA%\Claude\claude_desktop_config.json`  
**macOS:** `~/Library/Application Support/Claude/claude_desktop_config.json`

```json
{
  "mcpServers": {
    "daily-scrum": {
      "type": "stdio",
      "command": "dotnet",
      "args": ["run", "--project", "/absolute/path/to/daily-scrum/src/Mcp/Mcp.csproj"],
      "env": {
        "MSTODO__ACCESSTOKEN": "your-microsoft-graph-access-token-here"
      }
    }
  }
}
```

**Important:**
- Replace `/absolute/path/to/daily-scrum/` with the actual absolute path to your project
- Replace `your-microsoft-graph-access-token-here` with your Microsoft Graph access token
- Use forward slashes (`/`) in the path, even on Windows
- On Windows, the path might look like: `C:/Users/YourName/Code/daily-scrum/src/Mcp/Mcp.csproj`
- On macOS/Linux: `/Users/YourName/Code/daily-scrum/src/Mcp/Mcp.csproj`

After configuring, restart Claude Desktop for the changes to take effect.

## Project Architecture

This MCP server uses:
- **STDIO Transport** - Communicates with MCP clients via standard input/output
- **IOptions Pattern** - For configuration management (reading environment variables)
- **Dependency Injection** - For service management
- **Microsoft Graph SDK** - For accessing Microsoft To-Do API
- **Strongly Typed Models** - For response data structures

### Project Structure

- `Program.cs` - Main server configuration and dependency injection setup
- `Features/Tasks/` - Task-related tools and models
  - `GetTasksTool.cs` - Tool for retrieving tasks
  - `GetTasksResponse.cs` - Response models
  - `MsToDoOptions.cs` - Configuration options
- `Features/Identity/` - Authentication-related tools
  - `SetAccessTokenTool.cs` - Tool for setting access token
- `appsettings.json` - Application configuration
- `README.md` - This documentation

## Dependencies

- `ModelContextProtocol.AspNetCore` - MCP server framework for ASP.NET Core
- `Microsoft.Graph` - Microsoft Graph SDK for API access
- `Infrastructure` - Custom infrastructure layer with Graph service
- `Domain` - Domain models and entities

## Building

Build the project using:

```bash
dotnet build
```

## Testing

You can test the MCP server by:

1. Configuring it in Claude Desktop (see Configuration section above)
2. Restarting Claude Desktop
3. Asking Claude to retrieve your tasks

### Example Queries

After configuration, you can ask Claude:
- "What tasks did I work on yesterday?"
- "Show me my To-Do tasks from last week"
- "What are my tasks for today?"

Claude will use the GetTasks tool to retrieve your Microsoft To-Do tasks from the specified date range.

## Troubleshooting

### Access Token Issues

If you receive an error about missing access token:
1. Verify the environment variable `MSTODO__ACCESSTOKEN` is set correctly (note the double underscore)
2. Check that your access token is still valid (they typically expire after 1 hour)
3. Get a new token from [Microsoft Graph Explorer](https://developer.microsoft.com/en-us/graph/graph-explorer)

### Connection Issues

If Claude Desktop can't connect to the server:
1. Verify the path to `Mcp.csproj` is correct and absolute
2. Check that .NET 9.0 SDK is installed (`dotnet --version`)
3. Look at Claude Desktop logs for error messages
4. Try running `dotnet run --project /path/to/Mcp.csproj` manually to check for errors

## License

See the LICENSE file in the root of the repository.
