using Xunit;
using Moq;
using System.Collections.Generic;

namespace SpaceBattle.Tests;

public class RegisterIoCDependencyMacroMoveRotateTests
{
    public RegisterIoCDependencyMacroMoveRotateTests()
    {
        new RegisterIoCDependencyMacroCommand().Execute();

        new RegisterIoCDependencyMacroMoveRotate().Execute();
    }

    [Fact]
    public void MacroMove_ShouldResolveAndExecuteCommandsCorrectly()
    {
        var mockCommand1 = new Mock<ICommand>();
        var mockCommand2 = new Mock<ICommand>();

        Ioc.Resolve<ICommand>("IoC.Register", "Specs.Move",
            (object[] args) => new List<string> { "Move.Cmd1", "Move.Cmd2" }).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Move.Cmd1",
            (object[] args) => mockCommand1.Object).Execute();
        Ioc.Resolve<ICommand>("IoC.Register", "Move.Cmd2",
            (object[] args) => mockCommand2.Object).Execute();

        var macroMove = Ioc.Resolve<ICommand>("Macro.Move", new object[] { });
        macroMove.Execute();

        mockCommand1.Verify(c => c.Execute(), Times.Once());
        mockCommand2.Verify(c => c.Execute(), Times.Once());
    }

    [Fact]
    public void MacroRotate_ShouldResolveAndExecuteCommandsCorrectly()
    {
        var mockCommand1 = new Mock<ICommand>();
        var mockCommand2 = new Mock<ICommand>();

        Ioc.Resolve<ICommand>("IoC.Register", "Specs.Rotate",
            (object[] args) => new List<string> { "Rotate.Cmd1", "Rotate.Cmd2" }).Execute();

        Ioc.Resolve<ICommand>("IoC.Register", "Rotate.Cmd1",
            (object[] args) => mockCommand1.Object).Execute();
        Ioc.Resolve<ICommand>("IoC.Register", "Rotate.Cmd2",
            (object[] args) => mockCommand2.Object).Execute();

        var macroRotate = Ioc.Resolve<ICommand>("Macro.Rotate", new object[] { });
        macroRotate.Execute();

        mockCommand1.Verify(c => c.Execute(), Times.Once());
        mockCommand2.Verify(c => c.Execute(), Times.Once());
    }
}