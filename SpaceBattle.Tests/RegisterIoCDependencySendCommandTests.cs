using Moq;
using Xunit;
using SpaceBattle;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencySendCommandTests
{
    [Fact]
    public void Execute_RegisterIoCDependencySendCommand_DependencyResolves()
    {
        var mockReceiver = new Mock<ICommandReceiver>();

        new RegisterIoCDependencySendCommand().Execute();

        var cmd = Ioc.Resolve<ICommand>("Commands.Send", new Mock<ICommand>().Object, mockReceiver.Object);

        Assert.NotNull(cmd);
    }
}
