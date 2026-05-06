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
}