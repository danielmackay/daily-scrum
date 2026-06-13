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

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- A Microsoft account in the SSW tenant with Microsoft To-Do tasks
- The `daily-scrum` CLI installed (see [Authentication](#authentication))

## Authentication

The server reads Microsoft Graph tokens from a shared, encrypted cache that the
`daily-scrum` CLI fills in. You sign in once through the browser; after that the
server refreshes tokens silently for as long as the refresh token lasts (roughly
90 days). There is no token to copy or paste, and nothing to set in the MCP config.

### Install the CLI

Package and install the global tool from the repo root:

```bash
dotnet pack src/Cli -o ./nupkg
dotnet tool install -g --add-source ./nupkg DailyScrum.Cli
```

To upgrade after a code change, run `dotnet tool update -g --add-source ./nupkg DailyScrum.Cli`.

### Sign in

```bash
daily-scrum login     # opens the browser, then caches the refresh token
daily-scrum status    # confirms the server can authenticate silently
daily-scrum logout    # forgets the cached account on this machine
```

`login` writes two things. The refresh token goes into the macOS Keychain,
encrypted. A small account record goes to `~/.daily-scrum/auth-record.json`; it
holds no secret and only tells the server which cached account to use. You will
need to sign in again when the refresh token finally expires, and
`daily-scrum status` tells you when that has happened.

### Running the Server

The MCP client (e.g. Claude Desktop) starts the server for you. To run it by hand:

```bash
cd src/Mcp
dotnet run
```

The server starts and communicates over STDIO. As long as you have run
`daily-scrum login`, it picks up the cached credentials automatically.

## Using the MCP Server

The server is designed to be used through MCP clients like Claude Desktop. Once configured (see Configuration section), you can ask Claude to retrieve your tasks.

### Available Tools

#### GetTasks

Retrieves Microsoft To-Do tasks grouped by day and project/list.

**Parameters:**
- `localStart` (DateTime, required) - Start date in ISO 8601 format (e.g., 2025-10-29T00:00:00Z)
- `localEnd` (DateTime, required) - End date in ISO 8601 format (e.g., 2025-10-29T23:59:59Z)

**Returns:**
A JSON object containing tasks grouped by day, with each day containing projects and their associated tasks.

If you have not signed in, the tool returns `{ "Error": "Not authenticated. Run 'daily-scrum login' in a terminal, then try again." }`.

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
      "args": ["run", "--project", "/absolute/path/to/daily-scrum/src/Mcp/Mcp.csproj"]
    }
  }
}
```

**Important:**
- Replace `/absolute/path/to/daily-scrum/` with the actual absolute path to your project
- Use forward slashes (`/`) in the path, even on Windows
- On Windows, the path might look like: `C:/Users/YourName/Code/daily-scrum/src/Mcp/Mcp.csproj`
- On macOS/Linux: `/Users/YourName/Code/daily-scrum/src/Mcp/Mcp.csproj`
- No access token goes in the config; authentication comes from `daily-scrum login`

After configuring, restart Claude Desktop for the changes to take effect.

## Project Architecture

This MCP server uses:
- **STDIO Transport** - Communicates with MCP clients via standard input/output
- **Shared MSAL token cache** - Tokens acquired by the `daily-scrum` CLI, read silently by the server
- **Dependency Injection** - For service management
- **Microsoft Graph SDK** - For accessing Microsoft To-Do API
- **Strongly Typed Models** - For response data structures

### Project Structure

- `Program.cs` - Main server configuration and dependency injection setup
- `Features/Tasks/` - Task-related tools and models
  - `GetTasksTool.cs` - Tool for retrieving tasks
  - `GetTasksResponse.cs` - Response models
- `appsettings.json` - Application configuration
- `README.md` - This documentation

Authentication lives one layer down, shared with the CLI:
- `Infrastructure/Identity/GraphAuthenticator.cs` - Interactive login, silent credential, and the shared cache settings
- `src/Cli/` - The `daily-scrum` global tool (`login` / `status` / `logout`)

## Dependencies

- `ModelContextProtocol.AspNetCore` - MCP server framework for ASP.NET Core
- `Microsoft.Graph` - Microsoft Graph SDK for API access
- `Infrastructure` - Custom infrastructure layer with Graph service and authentication
- `Domain` - Domain models and entities

## Building

Build the project using:

```bash
dotnet build
```

## Testing

You can test the MCP server by:

1. Running `daily-scrum login`
2. Configuring it in Claude Desktop (see Configuration section above)
3. Restarting Claude Desktop
4. Asking Claude to retrieve your tasks

### Example Queries

After configuration, you can ask Claude:
- "What tasks did I work on yesterday?"
- "Show me my To-Do tasks from last week"
- "What are my tasks for today?"

Claude will use the GetTasks tool to retrieve your Microsoft To-Do tasks from the specified date range.

## Troubleshooting

### Authentication Issues

If `GetTasks` returns a "Not authenticated" error:
1. Run `daily-scrum status` to check whether the cached session is still valid
2. If it reports an expired or missing session, run `daily-scrum login` and sign in again
3. Confirm the same user account runs both the CLI and the MCP client — they share one cache

### Connection Issues

If Claude Desktop can't connect to the server:
1. Verify the path to `Mcp.csproj` is correct and absolute
2. Check that .NET 9.0 SDK is installed (`dotnet --version`)
3. Look at Claude Desktop logs for error messages
4. Try running `dotnet run --project /path/to/Mcp.csproj` manually to check for errors

## License

See the LICENSE file in the root of the repository.
