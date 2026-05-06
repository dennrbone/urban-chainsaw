using SpaceBattle;
using Xunit;
using Moq;

namespace SpaceBattle.Tests;

public class ActionsFullCoverageTests
{
    public ActionsFullCoverageTests()
    {
        RegisterIoCDependencyActionsStart._activeActions.Clear();
    }

    [Fact]
    public void ActionsStart_Success()
    {
        var mockQueue = new Mock<ICommandReceiver>();
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Queue", (Func<object[], object>)(a => mockQueue.Object)).Execute();
        Ioc.Resolve<ICommand>("IoC.Register", "Move", (Func<object[], object>)(a => new Mock<ICommand>().Object)).Execute();
        new RegisterIoCDependencyActionsStart().Execute();
        var order = new Dictionary<string, object> { { "ActionId", "1" }, { "GameObject", new object() }, { "CommandName", "Move" } };
        var startCmd = Ioc.Resolve<ICommand>("Actions.Start", order);
        startCmd.Execute();
        Assert.True(RegisterIoCDependencyActionsStart._activeActions.ContainsKey("1"));
    }

    [Fact]
    public void ActionsStart_Errors()
    {
        new RegisterIoCDependencyActionsStart().Execute();
        Assert.Throws<KeyNotFoundException>(() => Ioc.Resolve<ICommand>("Actions.Start", new Dictionary<string, object>()));
        Assert.Throws<InvalidCastException>(() => Ioc.Resolve<ICommand>("Actions.Start", new Dictionary<string, object> { { "ActionId", 1 } }));
    }

    [Fact]
    public void ActionsStop_Success()
    {
        var mockQueue = new Mock<ICommandQueue>();
        Ioc.Resolve<ICommand>("IoC.Register", "Game.Queue", (Func<object[], object>)(a => mockQueue.Object)).Execute();
        RegisterIoCDependencyActionsStart._activeActions["stop"] = new CommandInjectableCommand();
        new RegisterIoCDependencyActionsStop().Execute();
        var stopCmd = Ioc.Resolve<ICommand>("Actions.Stop", new Dictionary<string, object> { { "ActionId", "stop" } });
        stopCmd.Execute();
        Assert.False(RegisterIoCDependencyActionsStart._activeActions.ContainsKey("stop"));
    }

    [Fact]
    public void ActionsStop_Exception()
    {
        new RegisterIoCDependencyActionsStop().Execute();
        Assert.Throws<ArgumentException>(() => Ioc.Resolve<ICommand>("Actions.Stop", new Dictionary<string, object> { { "ActionId", "none" } }));
    }

    [Fact]
    public void Helpers_Coverage()
    {
        bool executed = false;
        new ActionCommand(() => executed = true).Execute();
        Assert.True(executed);

        var mockCmd = new Mock<ICommand>();
        var mockRec = new Mock<ICommandReceiver>();
        new SendCommand(mockCmd.Object, mockRec.Object).Execute();
        mockRec.Verify(r => r.Receive(mockCmd.Object), Times.Once);

        var injectable = new CommandInjectableCommand();
        injectable.Inject(mockCmd.Object);
        injectable.Execute();
        mockCmd.Verify(m => m.Execute(), Times.Once);
        Assert.Throws<InvalidOperationException>(() => new CommandInjectableCommand().Execute());
    }

    [Fact]
    public void IoC_Internal_Coverage()
    {
        Assert.Throws<InvalidOperationException>(() => Ioc.Resolve<object>("Unknown"));
        var startReg = new RegisterIoCDependencyActionsStart();
        var stopReg = new RegisterIoCDependencyActionsStop();
        startReg.Execute();
        stopReg.Execute();
    }
}