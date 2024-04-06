using Azure.Core;

namespace WebUI.Features.DailyScrum.Infrastructure;

internal class JwtTokenCredential : TokenCredential
{
    private readonly string _accessToken;

    public JwtTokenCredential(string accessToken)
    {
        _accessToken = accessToken;
    }

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new AccessToken(_accessToken, DateTimeOffset.Now.AddDays(1));
    }

    public override ValueTask<AccessToken> GetTokenAsync(
        TokenRequestContext requestContext,
        CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(GetToken(requestContext, cancellationToken));
    }
}
