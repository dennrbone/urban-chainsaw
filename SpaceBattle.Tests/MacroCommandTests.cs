using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using SpaceBattle;

namespace SpaceBattle.Tests;

public class MacroCommandTests
{
    [Fact]
    public void MacroCommand_ShouldExecuteAllCommandsInArray()
    {
        var cmd1 = new Mock<ICommand>();
        var cmd2 = new Mock<ICommand>();
        var commands = new[] { cmd1.Object, cmd2.Object };
        var macro = new MacroCommand(commands);

        macro.Execute();

        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());
    }

    [Fact]
    public void MacroCommand_ShouldStop_WhenAnyCommandThrowsException()
    {
        var cmd1 = new Mock<ICommand>();
        var cmd2 = new Mock<ICommand>();
        var cmd3 = new Mock<ICommand>();

        cmd1.Setup(c => c.Execute());
        cmd2.Setup(c => c.Execute()).Throws(new Exception("Command failed"));

        var commands = new[] { cmd1.Object, cmd2.Object, cmd3.Object };
        var macro = new MacroCommand(commands);


        Assert.ThrowsAny<Exception>(() => macro.Execute());

        cmd1.Verify(c => c.Execute(), Times.Once());
        cmd2.Verify(c => c.Execute(), Times.Once());
        cmd3.Verify(c => c.Execute(), Times.Never());
    }
}