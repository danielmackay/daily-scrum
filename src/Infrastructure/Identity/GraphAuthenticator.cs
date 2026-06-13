using Azure.Core;
using Azure.Identity;

namespace Infrastructure.Identity;

/// <summary>
/// Centralises Microsoft Graph delegated authentication for both the interactive
/// CLI (<c>daily-scrum login</c>) and the silent-only MCP server.
///
/// Both hosts build an <see cref="InteractiveBrowserCredential"/> over the same
/// named persistent cache (<see cref="CacheName"/>) and the same persisted
/// <see cref="AuthenticationRecord"/>. The CLI seeds the cache interactively; the
/// server reads it silent-only so it never pops a browser mid tool-call.
///
/// The refresh token lives encrypted in the OS secret store (macOS Keychain) via
/// <see cref="TokenCachePersistenceOptions"/>. The <see cref="AuthenticationRecord"/>
/// persisted to <see cref="AuthRecordPath"/> holds only account metadata (username,
/// home-account id, tenant, authority, client id) and no secret, so plain-file
/// storage is safe — it merely tells the silent credential which cached account to use.
/// </summary>
public static class GraphAuthenticator
{
    // App registration "DailyScrum" in the SSW tenant. Public client (native) flow.
    public const string ClientId = "2407f45c-4141-4484-8fc5-ce61327519d9";
    public const string TenantId = "ac2f7c34-b935-48e9-abdc-11e5d4fcb2b0";

    // Shared cache identity — must be identical in the CLI and the MCP server so
    // both processes resolve the same encrypted token cache.
    public const string CacheName = "daily-scrum";

    // Delegated, read-only scopes. offline_access (refresh token) is added
    // implicitly by MSAL for public clients, so it is not listed here.
    public static readonly string[] Scopes =
    {
        "Mail.Read",
        "Mail.ReadBasic",
        "MailboxFolder.Read",
        "MailboxItem.Read",
        "Tasks.Read",
        "User.Read",
    };

    /// <summary>
    /// Path to the persisted <see cref="AuthenticationRecord"/>. Identical in both
    /// hosts (same user, same machine) so they share one account selector.
    /// </summary>
    public static string AuthRecordPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".daily-scrum",
        "auth-record.json");

    /// <summary>
    /// Interactive sign-in. Pops the system browser once, then persists the
    /// resulting refresh token (Keychain) and account record (file). Run by the
    /// <c>daily-scrum login</c> CLI verb.
    /// </summary>
    public static async Task<AuthenticationRecord> LoginInteractiveAsync(
        CancellationToken cancellationToken = default)
    {
        var credential = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions
        {
            ClientId = ClientId,
            TenantId = TenantId,
            // Registered under "Mobile and desktop applications" as http://localhost;
            // MSAL appends a free loopback port at runtime.
            RedirectUri = new Uri("http://localhost"),
            TokenCachePersistenceOptions = new TokenCachePersistenceOptions { Name = CacheName },
        });

        var record = await credential.AuthenticateAsync(
            new TokenRequestContext(Scopes), cancellationToken);

        await PersistAuthRecordAsync(record, cancellationToken);
        return record;
    }

    /// <summary>
    /// Builds a silent-only credential for the MCP server. Reads the shared cache
    /// and the persisted account record; never triggers interactive UI. When no
    /// valid token can be acquired silently it throws
    /// <see cref="AuthenticationRequiredException"/> (or
    /// <see cref="CredentialUnavailableException"/>), which callers translate into
    /// a "run daily-scrum login" message.
    /// </summary>
    public static InteractiveBrowserCredential CreateSilentCredential()
    {
        var options = new InteractiveBrowserCredentialOptions
        {
            ClientId = ClientId,
            TenantId = TenantId,
            TokenCachePersistenceOptions = new TokenCachePersistenceOptions { Name = CacheName },
            // Silent-only: throw instead of opening a browser when interaction is needed.
            DisableAutomaticAuthentication = true,
        };

        var record = TryLoadAuthRecord();
        if (record is not null)
            options.AuthenticationRecord = record;

        return new InteractiveBrowserCredential(options);
    }

    /// <summary>
    /// Probes the silent path for the <c>status</c> CLI verb. Returns the acquired
    /// token (caller reads <see cref="AccessToken.ExpiresOn"/>), or null when no
    /// account is cached / re-login is required.
    /// </summary>
    public static async Task<AccessToken?> ProbeSilentAsync(CancellationToken cancellationToken = default)
    {
        if (TryLoadAuthRecord() is null)
            return null;

        try
        {
            var credential = CreateSilentCredential();
            return await credential.GetTokenAsync(new TokenRequestContext(Scopes), cancellationToken);
        }
        catch (Exception ex) when (ex is AuthenticationRequiredException or CredentialUnavailableException)
        {
            return null;
        }
    }

    /// <summary>
    /// Clears local sign-in state for the <c>logout</c> CLI verb by deleting the
    /// account record. Without the record the silent credential can no longer
    /// select a cached account; a fresh <c>login</c> overwrites the cache entirely.
    /// </summary>
    public static void ClearCache()
    {
        if (File.Exists(AuthRecordPath))
            File.Delete(AuthRecordPath);
    }

    /// <summary>Loads the persisted account record, or null when absent/unreadable.</summary>
    public static AuthenticationRecord? TryLoadAuthRecord()
    {
        if (!File.Exists(AuthRecordPath))
            return null;

        using var stream = File.OpenRead(AuthRecordPath);
        return AuthenticationRecord.Deserialize(stream);
    }

    private static async Task PersistAuthRecordAsync(
        AuthenticationRecord record, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(AuthRecordPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        await using var stream = File.Create(AuthRecordPath);
        await record.SerializeAsync(stream, cancellationToken);
    }
}
