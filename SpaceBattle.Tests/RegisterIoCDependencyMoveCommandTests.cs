using Xunit;
using Moq;
using SpaceBattle;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencyMoveCommandTests
{
    [Fact]
    public void Execute_RegisterIoCDependencyMoveCommand_DependencyResolves()
    {
        var mockMoving = new Mock<IMoving>();
        mockMoving.SetupProperty(m => m.Position, new NVector(12, 5));
        mockMoving.SetupGet(m => m.Velocity).Returns(new NVector(-4, 1));

        Ioc.Resolve<ICommand>(
            "IoC.Register",
            "Adapters.IMoving",
            (object[] args) => mockMoving.Object
        ).Execute();

        new RegisterIoCDependencyMoveCommand().Execute();

        var cmd = Ioc.Resolve<ICommand>("Commands.Move", new object());

        Assert.NotNull(cmd);
        cmd.Execute();
        Assert.Equal(new NVector(8, 6), mockMoving.Object.Position);
    }
}