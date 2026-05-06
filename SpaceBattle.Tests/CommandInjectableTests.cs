using Moq;
using SpaceBattle;
using Xunit;

public class CommandInjectableTests
{
    [Fact]
    public void Execute_ShouldInvokeInjectedCommand()
    {
        var mock = new Mock<ICommand>();
        var injectable = new CommandInjectableCommand();
        injectable.Inject(mock.Object);

        injectable.Execute();

        mock.Verify(m => m.Execute(), Times.Once);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenNoCommandInjected()
    {
        var injectable = new CommandInjectableCommand();
        Assert.Throws<InvalidOperationException>(() => injectable.Execute());
    }
}