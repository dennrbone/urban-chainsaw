using SpaceBattle;
using Xunit;
using Moq;

namespace SpaceBattle.Tests;

public class ActionsStartTests
{
    [Fact]
    public void ActionsStart_ShouldRegisterAndExecute_Correctly()
    {
        var mockQueue = new Mock<ICommandReceiver>();
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Queue", (Func<object[], object>)(a => mockQueue.Object)).Execute();
        Ioc.Resolve<ICommand>("IoC.Register", "Move", (Func<object[], object>)(a => new Mock<ICommand>().Object)).Execute();

        new RegisterIoCDependencyActionsStart().Execute();

        var order = new Dictionary<string, object>
        {
            { "ActionId", "act1" },
            { "GameObject", new object() },
            { "CommandName", "Move" }
        };

        var startCmd = Ioc.Resolve<ICommand>("Actions.Start", order);
        startCmd.Execute();

        Assert.True(RegisterIoCDependencyActionsStart._activeActions.ContainsKey("act1"));
        mockQueue.Verify(q => q.Receive(It.IsAny<ICommand>()), Times.Once);
    }
}