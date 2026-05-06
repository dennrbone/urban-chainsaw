using Xunit;
using Moq;
using System.Collections.Generic;
using SpaceBattle;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencyMacroCommandTests
{
    [Fact]
    public void Execute_RegisterIoCDependencyMacroCommand_DependencyResolves()
    {
        var cmd1 = new Mock<ICommand>();
        var cmd2 = new Mock<ICommand>();
        var commands = new List<ICommand> { cmd1.Object, cmd2.Object };

        new RegisterIoCDependencyMacroCommand().Execute();

        var macro = Ioc.Resolve<ICommand>("Commands.Macro", commands);

        Assert.NotNull(macro);
        macro.Execute();

        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());
    }
}