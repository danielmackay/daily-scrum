using FluentAssertions;
using Infrastructure.Identity;
using WebUI.Features.Identity;

namespace UnitTests;

public class UpdateAccessTokenCommandTests
{
    [Fact]
    public void Handle_GivenValidAccessToken_ShouldSetFirstNameAndLastName()
    {
        var currentUserService = new CurrentUserService();
        var sut = new UpdateAccessTokenCommandHandler(currentUserService);
        var token = "TOKEN";

        sut.Handle(new UpdateAccessTokenCommand(token), CancellationToken.None);

        currentUserService.UserName.Should().Be("Daniel Mackay");
    }
}
