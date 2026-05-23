using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceBattle.Tests;

public class CreateMacroCommandStrategyTests
{
    public CreateMacroCommandStrategyTests()
    {
        new RegisterIoCDependencyMacroCommand().Execute();
    }

    [Fact]
    public void Resolve_WhenMacroTestExistsAndCommandsExist_MacroCommandResolvesAndExecutesAllCommands()
    {
        var command1 = new Mock<ICommand>();
        var command2 = new Mock<ICommand>();

        Ioc.Resolve<ICommand>("IoC.Register", "Specs.Macro.Test",
            (object[] args) => new[] { "Test.Command1", "Test.Command2" }).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Test.Command1",
            (object[] args) => command1.Object).Execute();
        Ioc.Resolve<ICommand>("IoC.Register", "Test.Command2",
            (object[] args) => command2.Object).Execute();

        var strategy = new CreateMacroCommandStrategy("Specs.Macro.Test");

        var macroCommand = strategy.Resolve(new object[] { });
        macroCommand.Execute();

        command1.Verify(c => c.Execute(), Times.Once);
        command2.Verify(c => c.Execute(), Times.Once);
    }

    [Fact]
    public void Resolve_WhenSpecDoesNotExist_ThrowsException()
    {
        var strategy = new CreateMacroCommandStrategy("Specs.NonExistent");

        Assert.Throws<InvalidOperationException>(() => strategy.Resolve(new object[] { }));
    }

    [Fact]
    public void Resolve_WhenCommandDoesNotExist_ThrowsException()
    {
        Ioc.Resolve<ICommand>("IoC.Register", "Specs.Macro.Test",
            (object[] args) => new[] { "Test.Existing", "Test.NonExistent" }).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Test.Existing",
            (object[] args) => new Mock<ICommand>().Object).Execute();

        var strategy = new CreateMacroCommandStrategy("Specs.Macro.Test");

        Assert.Throws<InvalidOperationException>(() => strategy.Resolve(new object[] { }));
    }
}