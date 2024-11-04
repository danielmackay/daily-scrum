using Azure.Identity;
using Microsoft.Graph;
using WebUI.Common.Identity;

namespace WebUI.Features.DailyScrum.Infrastructure;

public class GraphServiceClientFactory
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IServiceProvider _serviceProvider;

    // TODO: Read from config
    private readonly string _clientId = "2407f45c-4141-4484-8fc5-ce61327519d9";
    private readonly string _tenantId = "ac2f7c34-b935-48e9-abdc-11e5d4fcb2b0";
    private readonly string _redirectUri = "http://localhost:5001"; // Must match redirect URI in app registration
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
