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

    [Fact]
    public void ActionsStart_ShouldThrowException_WhenOrderIsMissingFields()
    {
        new RegisterIoCDependencyActionsStart().Execute();

        var badOrder = new Dictionary<string, object>
        {
            { "ActionId", "error-id" },
            { "GameObject", new object() }
        };

        Assert.Throws<KeyNotFoundException>(() => Ioc.Resolve<ICommand>("Actions.Start", badOrder));
    }

    [Fact]
    public void ActionsStart_ShouldThrow_WhenOrderIsIncomplete()
    {
        new RegisterIoCDependencyActionsStart().Execute();
        var incompleteOrder = new Dictionary<string, object> { { "ActionId", "error" } };

        Assert.Throws<KeyNotFoundException>(() => Ioc.Resolve<ICommand>("Actions.Start", incompleteOrder));
    }

    [Fact]
    public void ActionsStart_ShouldThrow_WhenTypesAreInvalid()
    {
        new RegisterIoCDependencyActionsStart().Execute();
        var invalidOrder = new Dictionary<string, object>
        {
            { "ActionId", 123 }, // Ожидается string, передаем int
            { "GameObject", new object() },
            { "CommandName", "Move" }
        };

        Assert.Throws<InvalidCastException>(() => Ioc.Resolve<ICommand>("Actions.Start", invalidOrder));
    }

    [Fact]
    public void RegisterActionsStart_Execute_ShouldNotThrow()
    {
        var cmd = new RegisterIoCDependencyActionsStart();
        var exception = Record.Exception(() => cmd.Execute());
        Assert.Null(exception);
    }

    [Fact]
    public void IoC_Resolve_ShouldThrow_WhenDependencyIsUnknown()
    {
        Assert.Throws<InvalidOperationException>(() => Ioc.Resolve<object>("Unknown.Key"));
    }
}