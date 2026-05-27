using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using SpaceBattle;

namespace SpaceBattle.Tests;

public class AuthorizationTests
{
    [Fact]
    public void AuthorizationService_ValidPlayerAndAction_ShouldNotThrow()
    {
        var permissions = new List<(string, string)> { ("player123", "Fire") };
        var authService = new AuthorizationService(permissions);

        var exception = Record.Exception(() => authService.Authorize("player123", "Fire"));
        Assert.Null(exception);
    }

    [Fact]
    public void AuthorizationService_InvalidPlayer_ShouldThrowUnauthorizedException()
    {
        var permissions = new List<(string, string)> { ("player123", "Fire") };
        var authService = new AuthorizationService(permissions);

        Assert.Throws<UnauthorizedException>(() => authService.Authorize("intruder_id", "Fire"));
    }

    [Fact]
    public void FireCommand_WhenAuthorized_ExecutesSuccessfully()
    {
        var mockAuth = new Mock<IAuthorizationService>();
        mockAuth.Setup(a => a.Authorize("player1", "Fire")).Verifiable();

        var mockRepo = new Mock<IGameObjectRepository>();
        
        var shipMock = new Mock<IMoving>();
        shipMock.Setup(s => s.Velocity).Returns(new NVector(0, 0));
        shipMock.Setup(s => s.Position).Returns(new NVector(0, 0));

        var command = new FireCommand(shipMock.Object, new NVector(1, 0), mockRepo.Object, "player1", 5.0, cmd => {}, mockAuth.Object);

        command.Execute();

        mockAuth.Verify(a => a.Authorize("player1", "Fire"), Times.Once);
    }

    [Fact]
    public void FireCommand_WhenNotAuthorized_ThrowsUnauthorizedException()
    {
        var mockAuth = new Mock<IAuthorizationService>();
        mockAuth.Setup(a => a.Authorize("bad_player", "Fire"))
                .Throws(new UnauthorizedException("Access Denied"));

        var mockRepo = new Mock<IGameObjectRepository>();
        var shipMock = new Mock<IMoving>();

        var command = new FireCommand(shipMock.Object, new NVector(1, 0), mockRepo.Object, "bad_player", 5.0, cmd => {}, mockAuth.Object);

        Assert.Throws<UnauthorizedException>(() => command.Execute());
    }
}