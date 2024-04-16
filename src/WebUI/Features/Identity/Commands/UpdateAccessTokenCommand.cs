using MediatR;
using WebUI.Common.Identity;

namespace WebUI.Features.Identity.Commands;

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
        _currentUserService.UpdateAccessToken(request.AccessToken);
        return Task.CompletedTask;
    }
}
