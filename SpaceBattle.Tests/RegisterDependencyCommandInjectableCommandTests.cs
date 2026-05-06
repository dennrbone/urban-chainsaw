using SpaceBattle;
using Xunit;

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
}
