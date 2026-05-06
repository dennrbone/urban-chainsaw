using SpaceBattle;
using Xunit;
using Moq;

namespace SpaceBattle.Tests;

public class RegisterDependencyCommandInjectableCommandTests
{
    [Fact]
    public void IoC_ShouldResolveInjectable_WithoutExceptions_AsPerCriteria()
    {
        new RegisterDependencyCommandInjectableCommand().Execute();

        var cmd = Ioc.Resolve<ICommand>("Commands.CommandInjectable");
        Assert.NotNull(cmd);

        var injectable = Ioc.Resolve<ICommandInjectable>("Commands.CommandInjectable");
        Assert.NotNull(injectable);

        var concrete = Ioc.Resolve<CommandInjectableCommand>("Commands.CommandInjectable");
        Assert.NotNull(concrete);

        Assert.IsType<CommandInjectableCommand>(cmd);
    }

    [Fact]
    public void CommandInjectableCommand_ShouldThrowException_WhenCommandNotInjected()
    {
        var injectable = new CommandInjectableCommand();

        Assert.Throws<InvalidOperationException>(() => injectable.Execute());
    }

    [Fact]
    public void IoC_Resolve_ShouldThrowException_WhenDependencyNotFound()
    {
        Assert.Throws<InvalidOperationException>(() => Ioc.Resolve<object>("Unknown.Dependency"));
    }

    [Fact]
    public void RegisterCommand_ShouldOverwrite_WhenKeyAlreadyExists()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Test.Double", (Func<object[], object>)(a => 1)).Execute();
        Ioc.Resolve<ICommand>("IoC.Register", "Test.Double", (Func<object[], object>)(a => 2)).Execute();

        var result = Ioc.Resolve<int>("Test.Double");
        Assert.Equal(2, result);
    }
    [Fact]
    public void CommandInjectableCommand_ShouldExecuteInjectedCommand()
    {
        var injectable = new CommandInjectableCommand();
        var mockCommand = new Mock<ICommand>();
        
        injectable.Inject(mockCommand.Object);
        injectable.Execute();

        mockCommand.Verify(m => m.Execute(), Times.Once);
    }

    [Fact]
    public void IoC_Register_ShouldWorkCorrectly()
    {
        var testKey = "Test.Coverage.Key";
        Func<object[], object> strategy = args => "success";

        var regCmd = Ioc.Resolve<ICommand>("IoC.Register", testKey, strategy);
        regCmd.Execute();

        var result = Ioc.Resolve<string>(testKey);
        Assert.Equal("success", result);
    }

    [Fact]
    public void IoC_Resolve_WithEmptyArgs_ShouldNotThrow()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "EmptyArgsTest", (Func<object[], object>)(args => "ok")).Execute();
        
        var result = Ioc.Resolve<string>("EmptyArgsTest");
        Assert.Equal("ok", result);
    }
}
