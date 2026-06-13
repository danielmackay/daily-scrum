using Azure.Identity;
using Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Kiota.Authentication.Azure;

namespace Infrastructure.Graph;

public class GraphServiceClientFactory
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceProvider _serviceProvider;

    // TODO: Read from config
    private readonly string _clientId = "2407f45c-4141-4484-8fc5-ce61327519d9";
    private readonly string _tenantId = "ac2f7c34-b935-48e9-abdc-11e5d4fcb2b0";
    // private readonly string _redirectUri = "http://localhost:5001"; // Must match redirect URI in app registration
    private readonly string[] _scopes = new[] { "User.Read" };

    public GraphServiceClientFactory(ICurrentUserService currentUserService, IServiceProvider serviceProvider)
    {
        _currentUserService = currentUserService;
        _serviceProvider = serviceProvider;
    }

    public GraphServiceClient CreateDefault()
    {
        return _serviceProvider.GetRequiredService<GraphServiceClient>();
    }

    public GraphServiceClient CreateWithAccessToken()
    {
        var accessToken = _currentUserService.AccessToken;
        ArgumentException.ThrowIfNullOrEmpty(accessToken);
        var credential = new JwtTokenCredential(accessToken);
        return new GraphServiceClient(credential);
    }

    /// <summary>
    /// Builds a Graph client backed by the shared, persistent MSAL token cache
    /// (seeded by the <c>daily-scrum login</c> CLI). Acquires tokens silently and
    /// never prompts; throws <see cref="Azure.Identity.AuthenticationRequiredException"/>
    /// when the user needs to log in again.
    /// </summary>
    /// <summary>
    /// Builds a Graph client backed by the shared, persistent MSAL token cache
    /// (seeded by the <c>daily-scrum login</c> CLI). Acquires tokens silently and
    /// never prompts; throws <see cref="Azure.Identity.AuthenticationRequiredException"/>
    /// when the user needs to log in again.
    /// CAE is disabled (see below) so silent acquisition stays deterministic.
    /// </summary>
    public GraphServiceClient CreateWithCachedCredential()
    {
        // The Graph SDK enables CAE (Continuous Access Evaluation) by default, which forces
        // a CAE-capable token request. Our shared cache is seeded by a non-CAE interactive
        // login, and the silent credential has DisableAutomaticAuthentication = true — so a
        // CAE request makes MSAL demand interactive sign-in and throw
        // AuthenticationRequiredException instead of refreshing silently. This server is
        // silent-only and cannot service CAE claims challenges, so we disable CAE to keep
        // token requests aligned with the cached (non-CAE) token.
        var authProvider = new AzureIdentityAuthenticationProvider(
            GraphAuthenticator.CreateSilentCredential(),
            allowedHosts: null,
            observabilityOptions: null,
            isCaeEnabled: false,
            scopes: GraphAuthenticator.Scopes);

        return new GraphServiceClient(authProvider);
    }

    public GraphServiceClient CreateWithDeviceCodeFlow()
    {
        var options = new DeviceCodeCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            ClientId = _clientId,
            TenantId = _tenantId,
            // Callback function that receives the user prompt
            // Prompt contains the generated device code that user must
            // enter during the auth process in the browser
            DeviceCodeCallback = (code, cancellation) =>
            {
                Console.WriteLine(code.Message);
                return Task.FromResult(0);
            },
        };

        // https://learn.microsoft.com/dotnet/api/azure.identity.devicecodecredential
        var deviceCodeCredential = new DeviceCodeCredential(options);

        var graphClient = new GraphServiceClient(deviceCodeCredential, _scopes);

        return graphClient;
    }
}
