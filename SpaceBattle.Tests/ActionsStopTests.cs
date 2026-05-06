using SpaceBattle;
using Xunit;
using Moq;

namespace SpaceBattle.Tests;

public class ActionsStopTests
{
    [Fact]
    public void ActionsStop_ShouldWorkInConstantTime()
    {
        var mockQueue = new Mock<ICommandQueue>();
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Queue", (Func<object[], object>)(a => mockQueue.Object)).Execute();

        var actionId = "stop_me";
        RegisterIoCDependencyActionsStart._activeActions[actionId] = new CommandInjectableCommand();

        new RegisterIoCDependencyActionsStop().Execute();
        var stopCmd = Ioc.Resolve<ICommand>("Actions.Stop", new Dictionary<string, object> { { "ActionId", actionId } });

        stopCmd.Execute();

        Assert.False(RegisterIoCDependencyActionsStart._activeActions.ContainsKey(actionId));
        mockQueue.Verify(q => q.Receive(It.IsAny<ICommand>()), Times.Once);
    }

    [Fact]
    public void ActionsStop_ShouldThrowException_IfNotFound()
    {
        new RegisterIoCDependencyActionsStop().Execute();
        var order = new Dictionary<string, object> { { "ActionId", "non_existent" } };

        Assert.Throws<ArgumentException>(() =>
            Ioc.Resolve<ICommand>("Actions.Stop", order)
        );
    }
}