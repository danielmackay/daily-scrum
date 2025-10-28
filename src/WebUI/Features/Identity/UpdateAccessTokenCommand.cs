using Infrastructure.Identity;
using MediatR;
using System.IdentityModel.Tokens.Jwt;

namespace WebUI.Features.Identity;

public record UpdateAccessTokenCommand(string AccessToken) : IRequest;

public class UpdateAccessTokenCommandHandler : IRequestHandler<UpdateAccessTokenCommand>
{
    private readonly ICurrentUserService _currentUserService;

    public UpdateAccessTokenCommandHandler(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public Task Handle(UpdateAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(request.AccessToken);
        var givenName = jwtToken.Claims.First(claim => claim.Type == "given_name").Value;
        var familyName = jwtToken.Claims.First(claim => claim.Type == "family_name").Value;

        _currentUserService.UpdateAccessToken(request.AccessToken);
        _currentUserService.UpdateName(givenName, familyName);

        return Task.CompletedTask;
    }
}
