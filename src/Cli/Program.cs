using Azure.Identity;
using Infrastructure.Identity;

// daily-scrum: a tiny global tool that owns the interactive part of Microsoft Graph
// auth, so the MCP server only ever reads the shared, silent token cache.
//
//   daily-scrum login    sign in once via the browser (seeds the cache + record)
//   daily-scrum status    check whether silent token acquisition still works
//   daily-scrum logout    forget the cached account locally

var command = args.Length > 0 ? args[0].ToLowerInvariant() : string.Empty;

switch (command)
{
    case "login":
        return await LoginAsync();
    case "status":
        return await StatusAsync();
    case "logout":
        return Logout();
    default:
        PrintUsage();
        // Treat a bare invocation as a usage error so scripts can detect it.
        return string.IsNullOrEmpty(command) ? 1 : UnknownCommand(command);
}

static async Task<int> LoginAsync()
{
    try
    {
        var record = await GraphAuthenticator.LoginInteractiveAsync();
        Console.WriteLine($"✓ Logged in as {record.Username}");
        Console.WriteLine($"  Account record: {GraphAuthenticator.AuthRecordPath}");
        return 0;
    }
    catch (AuthenticationFailedException ex)
    {
        Console.Error.WriteLine($"✗ Login failed: {ex.Message}");
        return 1;
    }
}

static async Task<int> StatusAsync()
{
    var record = GraphAuthenticator.TryLoadAuthRecord();
    if (record is null)
    {
        Console.WriteLine("✗ Not logged in. Run 'daily-scrum login'.");
        return 1;
    }

    var token = await GraphAuthenticator.ProbeSilentAsync();
    if (token is null)
    {
        Console.WriteLine($"✗ Session for {record.Username} has expired. Run 'daily-scrum login'.");
        return 1;
    }

    // ExpiresOn is the short-lived access token's expiry. A successful silent probe
    // proves the (long-lived) refresh token still works and is renewed automatically,
    // so we report the silent path as healthy rather than fabricating a session length.
    var minutes = Math.Max(0, (int)(token.Value.ExpiresOn - DateTimeOffset.UtcNow).TotalMinutes);
    Console.WriteLine($"✓ Signed in as {record.Username}");
    Console.WriteLine($"  Silent token acquisition OK (access token valid ~{minutes} min; refreshed automatically).");
    return 0;
}

static int Logout()
{
    GraphAuthenticator.ClearCache();
    Console.WriteLine("✓ Logged out. The MCP server will report 'not authenticated' until you log in again.");
    return 0;
}

static int UnknownCommand(string command)
{
    Console.Error.WriteLine($"Unknown command: {command}");
    return 1;
}

static void PrintUsage()
{
    Console.WriteLine("daily-scrum — Microsoft Graph auth for the Daily Scrum MCP server");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  daily-scrum login     Sign in via the browser (once every ~90 days)");
    Console.WriteLine("  daily-scrum status    Check whether the MCP server can authenticate silently");
    Console.WriteLine("  daily-scrum logout    Forget the cached account on this machine");
}
